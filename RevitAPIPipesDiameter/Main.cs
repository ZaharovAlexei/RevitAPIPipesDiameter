using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIPipesDiameter
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            for (int i = 0; i < pipes.Count(); i++)
            {
                Pipe element = pipes[i];
                {
                    Parameter outerParameter = element.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                    Parameter innerParameter = element.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);

                    double outerDiameter = 0;
                    double innerDiameter = 0;

                    if (outerParameter.StorageType == StorageType.Double)
                        outerDiameter = UnitUtils.ConvertFromInternalUnits(outerParameter.AsDouble(), UnitTypeId.Millimeters);

                    if (innerParameter.StorageType == StorageType.Double)
                        innerDiameter = UnitUtils.ConvertFromInternalUnits(innerParameter.AsDouble(), UnitTypeId.Millimeters);

                    Parameter name = element.LookupParameter("Наименование");
                    if (name != null)
                    {
                        using (Transaction ts = new Transaction(doc, "Set parameters"))
                        {
                            ts.Start();
                            name.Set($"Труба {outerDiameter:F1}/{innerDiameter:F1}");
                            ts.Commit();
                        }

                    }

                }
            }

            return Result.Succeeded;
        }
    }
}
