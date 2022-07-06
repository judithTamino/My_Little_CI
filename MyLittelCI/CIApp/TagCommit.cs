using System;
using System.Threading.Tasks;

namespace CIApp
{
    internal class TagCommit
    {
        private GithubAPI github = new GithubAPI();
        private string currentCommit = "";

        public void TagAndUploadToGithub()
        {

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    bool isRepositoryChange = await CheckChangesToRepository();

                    if (isRepositoryChange)
                        CreateNewTag();

                    System.Threading.Thread.Sleep(30000);
                }
            });
        }

        public async Task<bool> CheckChangesToRepository()
        {
            string latestCommitSha = await github.GetCurrentCommit();

            if (currentCommit.Equals(""))
            {
                currentCommit = latestCommitSha;
                return false;
            }
            else if (!(currentCommit.Equals(latestCommitSha)))
            {
                currentCommit = latestCommitSha;
                return true;
            }

            return false;
        }

        private async void CreateNewTag()
        {
            string[] tag = await github.CreateTagObject(currentCommit);
            bool isTagCreated = await github.AppendTagToRepository(tag);

            if(isTagCreated)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("TAG CREATED SUCCSSEFULLY");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TAG CREATED UNSUCCSSEFULLY");
                Console.ResetColor();
            }
        }
    }
}
