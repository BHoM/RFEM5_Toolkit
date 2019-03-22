using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
//using BH.Adapter.Queries;
using BH.oM.Base;
using System.Runtime.InteropServices;
using rf = Dlubal.RFEM5;
using Dlubal.RFEM5;
using System.Reflection;

namespace BH.Adapter.RFEM
{

    public partial class RFEMAdapter : BHoMAdapter
    {
        public const string ID = "RFEM_id";
        //public ModelInfo modelInfo = null;

        /// <summary>level of RFEM application lock: 0=no lock - ready to return UI, 1=licence locked, 2=model locked and ready for modification </summary>
        private int lockLevel = 0;

        /***************************************************/
        /**** Constructor                               ****/
        /***************************************************/

        public RFEMAdapter(string filePath = "")
        {
            AdapterId = ID;
            //modelInfo = new ModelInfo(modelData);

            app = Marshal.GetActiveObject("RFEM5.Application") as rf.IApplication;

            app.LockLicense();//check if licence locking can be delayed untill interaction is needed, i.e. create element, reade elements, ...
            lockLevel = 1;

            if (string.IsNullOrWhiteSpace(filePath))
                model = app.GetActiveModel();
            else
                model = app.OpenModel(filePath);

            if (model == null)
                ErrorLog.Add("file does not exist or the active RFEM model is hidden / running headless");

            modelData = model.GetModelData() as IModelData2;//only this version of modeldata supports setting multiple elements at a time - this is not obvious from documentation!!
            if (modelData == null)
                ErrorLog.Add("This RFEM version does not support interface IModelData2.");

            //modelInfo = new ModelInfo(modelData);


            InitialiseDefaults();

        }

        public void GetTest(string text)
        {
            this.AdapterId = text;
        }

        private void InitialiseDefaults()
        {
            //need to create some defaults so they will be available for any CRUD operation
            //eg. default steel material with known material number
            //note this could cause some unexpected behaviour eg. using an existion model a material might be replaced with the new default!

            //TODO: potentially needed to build supportNodeDict and supportLineDict here to be able to associate supports when user works with nodes etc...

            modelData.PrepareModification();
            //very illogically the reccomendation is to delete all existing material before adding a new material... and this cannot be done in the same transaction
            for (int i = 0; i < modelData.GetMaterialCount(); i++)
            {
                rf.IMaterial iMat = modelData.GetMaterial(i, rf.ItemAt.AtIndex);
                iMat.Delete();
            }
            //exit edit mode
            modelData.FinishModification();

            //default material - steel
            modelData.PrepareModification();
            rf.Material mat = new rf.Material();
            mat.TextID = "NameID|Steel S 235@TypeID|STEEL@StandardID|DIN EN 1993-1-1-10";
            mat.No = 1;
            modelData.SetMaterial(mat);
            //modelData.FinishModification();

            //default cross section
            modelData.PrepareModification();
            rf.CrossSection section = new rf.CrossSection();
            section.TextID = "IPE 100";
            section.MaterialNo = 1;
            section.No = 1;
            modelData.SetCrossSection(section);
            modelData.FinishModification();

            ////TODO: read coordiante system settings and set the translation accordingly - user creates this on model setup (6 potential combinations)
            //rf.OrientationType zOrientation = model.GetGlobalZAxisOrientation();
            //rf.WorkPlane workPlane = model.GetWorkPlane();
            //if (!SetCoordinateConversion(zOrientation, workPlane))
            //    ErrorLog.Add("Could not find good conversion from the used coordinatesystem to a righthand, positive Z version");
        }

        //TODO: complete and move this method somewhere more appropiate
        private static bool SetCoordinateConversion(rf.OrientationType zOrientation, rf.WorkPlane workPlane)
        {
            rf.PlaneType workPlaneType = workPlane.Type;

            switch (workPlaneType)
            {
                case rf.PlaneType.UnknownPlane:
                    return false;
                case rf.PlaneType.PlaneXY:
                    if (zOrientation == rf.OrientationType.Upward)
                        //set soordinate convertion = none
                        return true;
                    else
                        //do something else
                        return true;
                case rf.PlaneType.PlaneYZ:
                    //not sure yet
                    return false;
                case rf.PlaneType.PlaneXZ:
                    //not sure yet
                    return false;
                default:
                    return false;
            }
        }



        #region Pull/Push overrides to lock/unlock RFEM app

        public override bool Push(IEnumerable<BHoMObject> objects, string tag = "", Dictionary<string, string> config = null)
        {
            if (lockLevel == 0)
            {
                app.LockLicense();
                modelData.PrepareModification();
                lockLevel = 2;
            }
            if (lockLevel == 1)
            {
                modelData.PrepareModification();
                lockLevel = 2;
            }



            bool success = true;
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                success &= Replace(list as dynamic, tag);
            }

            if (lockLevel == 2)
            {
                modelData.FinishModification(true);
                app.UnlockLicense();
                lockLevel = 0;
            }
            if (lockLevel == 1)
            {
                app.UnlockLicense();
                lockLevel = 0;
            }


            return success;
        }

        /***************************************************/

        public override IEnumerable<object> Pull(IQuery query, Dictionary<string, string> config = null)
        {
            if (lockLevel == 0)
            {
                app.LockLicense();
                modelData.PrepareModification();
                lockLevel = 2;
            }
            if (lockLevel == 1)
            {
                modelData.PrepareModification();
                lockLevel = 2;
            }

            // Make sure this is a FilterQuery
            FilterQuery filter = query as FilterQuery;
            if (filter == null)
                return new List<object>();

            // Read the objects
            IEnumerable<BHoMObject> objectsRead = Read(filter.Type, filter.Tag);

            if (lockLevel == 2)
            {
                modelData.FinishModification(true);
                app.UnlockLicense();
                lockLevel = 0;
            }
            if (lockLevel == 1)
            {
                app.UnlockLicense();
                lockLevel = 0;
            }


            return objectsRead;
        }


        #endregion

    }


}
