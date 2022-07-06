using System;
using System.Collections.Generic;
using System.IO;

namespace CIApp
{
    internal class UploadToGit
    {
        private GithubAPI github = new GithubAPI();
        private const string REPO_NAME = "Demo_Project";

        public async void UploadFiles(string path)
        {
            string[] repoUrl = SliceRepoUrl(path);
            string githubPath = ConstructGithubPath(repoUrl);
            string encodedFile = ConvertToBase64String(path);

            string fileSha = await github.CreateBlob(encodedFile);

            string currentCommitSha = await github.GetCurrentCommit();
            string baseTreeSha = await github.GetBaseTree(currentCommitSha);

            string newTreeSha = await github.CreateTree(baseTreeSha, fileSha, githubPath);
            string newCommitSha = await github.AddCommit(newTreeSha, currentCommitSha);

            github.UpdateRepository(newCommitSha);
        }

        private string[] SliceRepoUrl(string path)
        {
            int repoNameIndex = path.IndexOf(REPO_NAME);
            return path.Substring(repoNameIndex).Split(new char[] { '\\' });
        }

        private string ConstructGithubPath(string[] repoURL)
        {
            string[] slicedURL = new string[repoURL.Length - 1];
            Array.Copy(repoURL, 1, slicedURL, 0, slicedURL.Length);
            string path = "";

            for (int i = 0; i < slicedURL.Length; i++)
                path += slicedURL[i] + "/";

            return path.Remove(path.Length - 1);
        }

        private string ConvertToBase64String(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
    }
}
