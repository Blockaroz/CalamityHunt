using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

public struct TextureAsset
{
    public string Path { get; private set; }
    public Texture2D Texture { get; private set; }

    public TextureAsset(string path)
    {
        Path = path;
        Texture = ModContent.Request<Texture2D>(Path, AssetRequestMode.ImmediateLoad).Value;
    }

    public static TextureAsset[] LoadArray(string path, int count, int start = 0)
    {
        TextureAsset[] assets = new TextureAsset[count - start];
        for (int i = 0; i < assets.Length; i++)
            assets[i] = new TextureAsset(path + (i + start));
        return assets;
    }

    public static implicit operator string(TextureAsset asset) => asset.Path;
    public static implicit operator Texture2D(TextureAsset asset) => asset.Texture;
}
