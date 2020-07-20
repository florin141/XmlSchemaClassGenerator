using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Xunit;
using System.Text;

namespace XmlSchemaClassGenerator.Tests
{
    [TestCaseOrderer("XmlSchemaClassGenerator.Tests.PriorityOrderer", "XmlSchemaClassGenerator.Tests")]
    public class Aixm
    {
        private Dictionary<NamespaceKey, string> _xsdToCsharpNsMap;

        static Aixm()
        {
            // Ensure that the output directories are empty.
            Directory.Delete(GetOutputPath(string.Empty), true);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact, TestPriority(1)]
        public void AirspaceServicesTest1()
        {
            var outputPath = GetOutputPath("AixmTest1");

            #region Namespaces
            
            string xlink = 			"http://www.w3.org/1999/xlink";
            string gml3 = 			"http://www.opengis.net/gml/3.2";
            string gts = 			"http://www.isotc211.org/2005/gts";
            string gss = 			"http://www.isotc211.org/2005/gss";
            string gsr = 			"http://www.isotc211.org/2005/gsr";
            string gmd = 			"http://www.isotc211.org/2005/gmd";
            string gco = 			"http://www.isotc211.org/2005/gco";

            string fixmBase = 		"http://www.fixm.aero/base/4.1";
            string fixmFlight = 	"http://www.fixm.aero/flight/4.1";
            string fixmNm = 		"http://www.fixm.aero/nm/1.2";
            string fixmMessaging = 	"http://www.fixm.aero/messaging/4.1";

            string adr = 			"http://www.aixm.aero/schema/5.1.1/extensions/EUR/ADR";
            string aixmV511 = 		"http://www.aixm.aero/schema/5.1.1";

            string adrmessage = 	"http://www.eurocontrol.int/cfmu/b2b/ADRMessage";

             _xsdToCsharpNsMap = new Dictionary<NamespaceKey, string>
            {
                { new NamespaceKey(), "other" },
                { new NamespaceKey(xlink), "org.w3._1999.xlink" },
                { new NamespaceKey(gts), "org.isotc211._2005.gts" },
                { new NamespaceKey(gss), "org.isotc211._2005.gss" },
                { new NamespaceKey(gsr), "org.isotc211._2005.gsr" },
                { new NamespaceKey(gmd), "org.isotc211._2005.gmd" },
                { new NamespaceKey(gco), "org.isotc211._2005.gco" },
                { new NamespaceKey(gml3), "net.opengis.gml._3" },
                { new NamespaceKey(aixmV511), "aixm.v5_1_1" },
                { new NamespaceKey(fixmNm), "aero.fixm.v4_1_0.nm.v1_2" },
                { new NamespaceKey(fixmMessaging), "aero.fixm.v4_1_0.messaging" },
                { new NamespaceKey(fixmFlight), "aero.fixm.v4_1_0.flight" },
                { new NamespaceKey(fixmBase), "aero.fixm.v4_1_0.base" },
                { new NamespaceKey(adr), "aero.aixm.schema._5_1_1.extensions.eur.adr" },
                { new NamespaceKey(adrmessage), "_int.eurocontrol.cfmu.b2b.adrmessage" }
            };

            #endregion

            var gen = new Generator
            {
                OutputFolder = outputPath,
                SeparateClasses = true,
                NamespaceProvider = _xsdToCsharpNsMap.ToNamespaceProvider(),
            };
            var xsdFiles = new[]
            {
                    "AIXM_AbstractGML_ObjectTypes.xsd",
                    "AIXM_DataTypes.xsd",
                    "AIXM_Features.xsd",
                    "extensions\\ADR-23.5.0\\ADR_DataTypes.xsd",
                    "extensions\\ADR-23.5.0\\ADR_Features.xsd",
                    "message\\ADR_Message.xsd",
                    "message\\AIXM_BasicMessage.xsd",
            }.Select(x => Path.Combine(InputPath, x)).ToList();
            var encodings = System.Text.Encoding.GetEncodings();
            System.Text.Encoding.GetEncoding("ISO-8859-15");
            gen.Generate(xsdFiles);
        }

        [Fact, TestPriority(2)]
        public void CanCompileClasses()
        {
            var inputPath = GetOutputPath("AixmTest1");
            var fileNames = new DirectoryInfo(inputPath).GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
            var assembly = Compiler.CompileFiles("Aixm.Test", fileNames);

            foreach (var ns in _xsdToCsharpNsMap.Values)
            {
                assembly.GetType(ns, true);
            }
        }

        private static string InputPath
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "xsd", "aixm\\aixm-5.1.1"); }
        }

        private static string GetOutputPath(string testCaseId)
        {
            var result = Path.Combine(Directory.GetCurrentDirectory(), "output", "Aixm", testCaseId);
            Directory.CreateDirectory(result);
            return result;
        }
    }
}
