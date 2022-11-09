namespace Depth_Camera
{
    public class FolderBrowser
    {
        public string ChooseFolder(string description)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Tag = description;
                fbd.UseDescriptionForTitle = true;
                fbd.Description = description;

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return fbd.SelectedPath;
                }
            }

            return string.Empty;
        }
    }
}
