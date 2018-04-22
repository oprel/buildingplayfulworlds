namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffectShader : SceneDemo
    {
        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            FES.EffectReset();

            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Decoration", 0);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Terrain", 1);

            FES.ShaderSetup(0, "Demos/DemoReel/PresentRippleShader");
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            FES.EffectReset();
            FES.MapClear();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Clear(22);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(22));
            }

            FES.DrawMapLayer(0);
            FES.DrawMapLayer(1);

            FES.EffectShader(0);
            FES.ShaderFloatSet(0, "Wave", FES.Ticks / 25.0f);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(new Rect2i(0, 0, FES.DisplaySize.width, 200), 1);
            }
            else
            {
                FES.DrawRectFill(new Rect2i(0, 0, FES.DisplaySize.width, 200), DemoUtil.IndexToRGB(1));
            }

            string shaderName = "PresentRippleShader";

            string codeStr = DemoUtil.HighlightCode("@C// Custom post-processing shader\n" +
                "@MFES@N.ShaderSetup(@L0@N, @S\"Demos/DemoReel/" + shaderName + "\"@N);\n" +
                "\n" +
                "@MFES@N.DrawMapLayer(@L0@N);\n" +
                "@MFES@N.DrawMapLayer(@L1@N);\n" +
                "\n" +
                "@MFES@N.EffectShader(@L0@N);\n" +
                "@MFES@N.ShaderFloatSet(@L0@N, @S\"Wave\"@N, " + (FES.Ticks / 25.0f) + ");\n" +
                "@MFES@N.EffectFilter(@MFES@N.@MFilter@N.Smooth);\n" +
                "\n" +
                "@MFES@N.EffectApplyNow();\n" +
                "@MFES@N.EffectReset();\n");

            string shaderStr = DemoUtil.HighlightCode("@C// This shader multiplies in a mask and applies a wavy effect!\n" +
                "@KShader@N \"Unlit/" + shaderName + "\" {\n" +
                "  @KSubShader@N {\n" +
                "    @C...\n" +
                "    @KPass@N {\n" +
                "      @C...\n" +
                "      @C/*** Insert custom shader variables here ***/\n" +
                "      @Kfloat@N Wave;\n" +
                "\n" +
                "      @Nfrag_in vert(appdata v) {\n" +
                "        @C...@N\n" +
                "      }\n" +
                "\n" +
                "      @Kfloat4@N frag(v2f i) : @MSV_Target@N {\n" +
                "        @C/*** Insert custom fragment shader code here ***/@N\n" +
                "        @Kfloat2@N centerOffset = @L-1.0@N + @L2.0@N * i.uv.xy;\n" +
                "        @Kfloat@N len = @Klength@N(centerOffset);\n" +
                "        i.uv.xy += (centerOffset / len) * cos(len * @L10.0@N - Wave) * @L0.005@N;\n" +
                "        @C...@N\n" +
                "        @Kreturn@N color;\n" +
                "      }\n" +
                "    }\n" +
                "  }\n" +
                "}\n");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(4, 4), 0, codeStr);
                FES.Print(new Vector2i(304, 4), 0, shaderStr);
            }
            else
            {
                FES.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(0), codeStr);
                FES.Print(new Vector2i(304, 4), DemoUtil.IndexToRGB(0), shaderStr);
            }
        }
    }
}
