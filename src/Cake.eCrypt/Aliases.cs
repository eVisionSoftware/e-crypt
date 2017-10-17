namespace Cake.eCrypt
{
    using Core;
    using Core.Annotations;
    using Common.Diagnostics;

    [CakeAliasCategory("Encryption")]
    public static class EncryptionAliases
    {
        [CakeMethodAlias]
        public static void EncryptPackage(this ICakeContext ctx, string target, string keyPath, string outputPath)
        {
            ctx.Information($"Encrypting package from {target} using public key from file {keyPath}");
            new EncryptWrapper(ctx.Information, ctx.Error).Encrypt(target, keyPath, outputPath);
        }
    }
}
