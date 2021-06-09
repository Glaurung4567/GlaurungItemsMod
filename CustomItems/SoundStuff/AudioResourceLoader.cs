using System.Reflection;

namespace GlaurungItems
{
    public class AudioResourceLoader
    {
        public static readonly string ResourcesDirectoryName = "GlaurungItemsMod";//"29406_Glaurung Items Mod (Sprites by Some Bunny)_1.8";
        public static readonly string AutoprocessDirectoryName = "Mods/" + ResourcesDirectoryName;
        public static readonly string AutoprocessModPathName = "Mods/" + ResourcesDirectoryName;
        public static readonly string ResourcesAutoprocessDirectoryName = AutoprocessDirectoryName;

        public static readonly string pathzip = GlaurungItems.ZipFilePath;
        public static readonly string pathfile = GlaurungItems.FilePath;


        public static void InitAudio()
        {
            //Tools.Print(pathzip, "ffffff", true);
            LoadAllAutoloadResourcesFromModPath(pathzip);
            // LoadAllAutoloadResourcesFromAssembly(Assembly.GetExecutingAssembly(), "ExpandTheGungeon");

            // LoadAllAutoloadResourcesFromPath(FullPathAutoprocess, "ExpandTheGungeon");
        }

        public static void LoadAllAutoloadResourcesFromAssembly(Assembly assembly, string prefix) {
            // this.LoaderText.AutoloadFromAssembly(assembly, prefix);
            // this.LoaderSprites.AutoloadFromAssembly(assembly, prefix, textureSize);
            ResourceLoaderSoundbanks LoaderSoundbanks = new ResourceLoaderSoundbanks();
            LoaderSoundbanks.AutoloadFromAssembly(assembly, prefix);
		}
        
		public static void LoadAllAutoloadResourcesFromPath(string path, string prefix) {
            // this.LoaderText.AutoloadFromPath(path, prefix);
            // this.LoaderSprites.AutoloadFromPath(path, prefix, textureSize);
            ResourceLoaderSoundbanks LoaderSoundbanks = new ResourceLoaderSoundbanks();
            LoaderSoundbanks.AutoloadFromPath(path, prefix);
		}

        public static void LoadAllAutoloadResourcesFromModPath(string path)
        {
            ResourceLoaderSoundbanks LoaderSoundbanks = new ResourceLoaderSoundbanks();
            LoaderSoundbanks.AutoloadFromModZIPOrModFolder(path);
        }

    }
}
