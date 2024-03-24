using PlainBuffers.CodeGen;
using PlainBuffers.CodeGen.Data;
using PlainBuffers.Generators;

namespace Frog.Level.CodeGen
{
    public class FrogUnityCodeGenerator : CSharpUnityCodeGenerator
    {
        public FrogUnityCodeGenerator(string[] namespaces) : base(namespaces)
        {
        }

        protected override void WriteArrayFields(CodeGenArray arrayType, BlockWriter typeBlock)
        {
        }
    }
}