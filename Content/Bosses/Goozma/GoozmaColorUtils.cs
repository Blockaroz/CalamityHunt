using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public static class GoozmaColorUtils
    {
        public static Vector3[] Oil = new Vector3[] //regular
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 46, 78).ToVector3(),
            new Color(113, 53, 146).ToVector3(),
            new Color(174, 23, 189).ToVector3(),
            new Color(237, 128, 60).ToVector3(),
            new Color(247, 255, 101).ToVector3(),
            new Color(176, 234, 85).ToVector3(),
            new Color(102, 219, 249).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Nuclear = new Vector3[] //nuclear throne
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(22, 81, 12).ToVector3(),
            new Color(87, 87, 87).ToVector3(),
            new Color(0, 236, 74).ToVector3(),
            new Color(66, 255, 176).ToVector3(),
            new Color(255, 255, 20).ToVector3(),
            new Color(186, 255, 0).ToVector3(),
            new Color(55, 255, 28).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };        
        
        public static Vector3[] Gold = new Vector3[] //obvious
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(23, 16, 9).ToVector3(),
            new Color(40, 28, 15).ToVector3(),
            new Color(213, 150, 78).ToVector3(),
            new Color(242, 192, 100).ToVector3(),
            new Color(255, 255, 147).ToVector3(),
            new Color(143, 99, 52).ToVector3(),
            new Color(111, 80, 41).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Grayscale = new Vector3[] //obvious 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 51, 51).ToVector3(),
            new Color(87, 87, 87).ToVector3(),
            new Color(120, 120, 120).ToVector3(),
            new Color(153, 153, 153).ToVector3(),
            new Color(235, 235, 235).ToVector3(),
            new Color(200, 200, 200).ToVector3(),
            new Color(187, 187, 187).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };
                
        public static Vector3[] Honey = new Vector3[] //honey; not the bees
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 4, 19).ToVector3(),
            new Color(135, 58, 6).ToVector3(),
            new Color(154, 255, 24).ToVector3(),
            new Color(216, 93, 10).ToVector3(),
            new Color(216, 163, 24).ToVector3(),
            new Color(150, 150, 24).ToVector3(),
            new Color(190, 85, 143).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };        
        
        public static Vector3[] Masterful = new Vector3[] //master mode rarity; for the worthy
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(200, 20, 10).ToVector3(),
            new Color(235, 80, 20).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(255, 150, 0).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(235, 80, 0).ToVector3(),
            new Color(220, 20, 10).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] DarkEnergy = new Vector3[] //dark bowser 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(49, 82, 156).ToVector3(),
            new Color(65, 98, 189).ToVector3(),
            new Color(98, 123, 197).ToVector3(),
            new Color(189, 164, 246).ToVector3(),
            new Color(148, 106, 246).ToVector3(),
            new Color(115, 65, 222).ToVector3(),
            new Color(64, 24, 180).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Frigid = new Vector3[] //white and blue; formerly squid girl
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(43, 109, 196).ToVector3(),
            new Color(100, 204, 239).ToVector3(),
            new Color(141, 229, 239).ToVector3(),
            new Color(239, 255, 253).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(224, 255, 236).ToVector3(),
            new Color(59, 186, 249).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Tritanopic = new Vector3[] //tritanopia 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(113, 33, 140).ToVector3(),
            new Color(244, 85, 41).ToVector3(),
            new Color(244, 34, 41).ToVector3(),
            new Color(244, 85, 90).ToVector3(),
            new Color(213, 244, 255).ToVector3(),
            new Color(71, 255, 255).ToVector3(),
            new Color(73, 209, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Caliginous = new Vector3[] //idk who this is
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(54, 16, 54).ToVector3(),
            new Color(99, 22, 130).ToVector3(),
            new Color(135, 102, 143).ToVector3(),
            new Color(164, 169, 163).ToVector3(),
            new Color(236, 62, 2).ToVector3(),
            new Color(165, 42, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Spectrum = new Vector3[] //zx spectrum console
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 0, 255).ToVector3(),
            new Color(255, 0, 0).ToVector3(),
            new Color(255, 0, 255).ToVector3(),
            new Color(0, 255, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(255, 255, 0).ToVector3(),
            new Color(0, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Evil = new Vector3[] //red and purple; maybe touch this one up
        {
            new Color(251, 233, 254).ToVector3(),
            new Color(210, 168, 249).ToVector3(),
            new Color(156, 139, 219).ToVector3(),
            new Color(156, 93, 190).ToVector3(),
            new Color(106, 66, 150).ToVector3(),
            new Color(103, 48, 62).ToVector3(),
            new Color(335, 87, 48).ToVector3(),
            new Color(38, 30, 50).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Shadowflame = new Vector3[] //shadowflame
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(69, 41, 150).ToVector3(),
            new Color(90, 24, 240).ToVector3(),
            new Color(181, 25, 248).ToVector3(),
            new Color(182, 141, 255).ToVector3(),
            new Color(251, 202, 255).ToVector3(),
            new Color(72, 102, 218).ToVector3(),
            new Color(69, 41, 150).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Dogmatic = new Vector3[] //dogma static
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] FIREBLU = new Vector3[] //FIREBLU
        {
            new Color(40, 0, 30).ToVector3(),
            new Color(71, 16, 10).ToVector3(),
            new Color(143, 53, 46).ToVector3(),
            new Color(194, 23, 89).ToVector3(),
            new Color(255, 38, 30).ToVector3(),
            new Color(15, 05, 01).ToVector3(),
            new Color(154, 164, 235).ToVector3(),
            new Color(22, 55, 255).ToVector3(),
            new Color(0, 0, 26).ToVector3()
        };

        public static Vector3[] Distorted = new Vector3[] //noxus palette (5/27/2023)
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(91, 69, 143).ToVector3(),
            new Color(123, 75, 230).ToVector3(),
            new Color(181, 91, 230).ToVector3(),
            new Color(110, 153, 255).ToVector3(),
            new Color(114, 218, 249).ToVector3(),
            new Color(110, 153, 255).ToVector3(),
            new Color(123, 75, 230).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Electric = new Vector3[] //blue and yellow
        {
            new Color(40, 0, 30).ToVector3(),
            new Color(255, 255, 100).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(355, 355, 289).ToVector3(),
            new Color(255, 238, 130).ToVector3(),
            new Color(15, 05, 01).ToVector3(),
            new Color(254, 234, 235).ToVector3(),
            new Color(126, 125, 25).ToVector3(),
            new Color(33, 30, 226).ToVector3()
        };

        public static Vector3[] Polterplasmic = new Vector3[] //polterghart
        {
            new Color(10, 10, 10).ToVector3(),
            new Color(15, 15, 100).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(13, 13, 13).ToVector3(),
            new Color(105, 105, 113).ToVector3(),
            new Color(122, 122, 144).ToVector3(),
            new Color(155, 255, 255).ToVector3(),
            new Color(133, 15, 25).ToVector3(),
            new Color(13, 13, 13).ToVector3()
        };

        public static Vector3[] Exhumed = new Vector3[] //red purple something?? idk
        {
            new Color(10, 10, 10).ToVector3(),
            new Color(115, 15, 140).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(142, 13, 13).ToVector3(),
            new Color(105, 105, 113).ToVector3(),
            new Color(122, 122, 144).ToVector3(),
            new Color(255, 155, 255).ToVector3(),
            new Color(113, 15, 145).ToVector3(),
            new Color(13, 13, 13).ToVector3()
        };

        public static Vector3[] Raptured = new Vector3[] //glitchy
        {
            new Vector3(-100f),
            new Vector3(0f),
            new Vector3(-0.2f, 0.1f, 0.4f),
            new Vector3(0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0.2f, 0.4f),
            new Vector3(0f),
            new Vector3(0.2f, 0.8f, 0.9f),
            new Vector3(1f, 1f, 0.4f)
        };

        public static Vector3[] Ibanical = new Vector3[] //blue and red again for the third time
        {
            new Vector3(0f),
            new Vector3(0f),
            new Vector3(-0.2f, 0.1f, 0.4f),
            new Vector3(0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0.2f, 0.4f),
            new Vector3(0f),
            new Vector3(0.2f, 0.8f, 0.9f),
            new Vector3(0f, 1f, 1f)
        };

        public static Vector3[] AcidRainbow = new Vector3[] //acidic rainbow
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(11, 0, 0).ToVector3(),
            new Color(15, 255, 255).ToVector3(),
            new Color(40, 0, 250).ToVector3(),
            new Color(255, 0, 250).ToVector3(),
            new Color(255, 20, 10).ToVector3(),
            new Color(244, 244, 0).ToVector3(),
            new Color(10, 256, 0).ToVector3(),
            new Color(15, 0, 0).ToVector3()
        };

        public static Vector3[] Rubicon = new Vector3[] //ruby con
        {
            new Color(255, 0, 0).ToVector3(),
            new Color(215, 145, 100).ToVector3(),
            new Color(55, 0, 0).ToVector3(),
            new Color(142, 13, 13).ToVector3(),
            new Color(200, 0, 0).ToVector3(),
            new Color(255, 50, 80).ToVector3(),
            new Color(155, 20, 50).ToVector3(),
            new Color(155, 0, 0).ToVector3(),
            new Color(55, 0, 0).ToVector3()
        };

        public static Vector3[] DoubleRainbow = new Vector3[] //colorshift
        {
            new Color(0, 0, 0).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.01f) % 1f, 0.4f, 0.4f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.1f) % 1f, 0.6f, 0.5f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.2f) % 1f, 0.9f, 0.6f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.5f) % 1f, 1f, 0.7f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.6f) % 1f, 0.9f, 0.6f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.7f) % 1f, 0.6f, 0.5f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.9f) % 1f, 0.4f, 0.4f).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Test => new Vector3[]
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(200, 20, 10).ToVector3(),
            new Color(235, 80, 20).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(255, 150, 0).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(235, 80, 0).ToVector3(),
            new Color(220, 20, 10).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };
    }
}
