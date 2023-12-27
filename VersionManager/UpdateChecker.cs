using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SoR4_Studio.VersionManager;

internal static class UpdateChecker
{
    private const string OWNER = "Zacksony";

    private const string REPOSITORY_NAME = "SoR4-Studio";

    public static async Task<string?> Check()
    {
        try
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            GitHubClient client = new(new ProductHeaderValue(OWNER));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(OWNER, REPOSITORY_NAME);

            //Setup the versions
            string? LatestReleaseTagName = null;

            foreach (Release release in releases)
            {
                if (!CurrentVersion.Instance.IsPreRelease && release.Prerelease)
                {
                    continue;
                }
                else
                {
                    LatestReleaseTagName = release.TagName;
                    break;
                }
            }

            if (LatestReleaseTagName is null)
            {
                return null;
            }

            Version latestGitHubVersion = new(PickoutVersionString(LatestReleaseTagName));
            Version localVersion = new(CurrentVersion.Instance.AppVersion);
            int? latestPreReleaseNumber = PickoutPreReleaseNumber(LatestReleaseTagName);

            //Compare the Versions
            //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);

            //The version on GitHub is more up to date than this local release.
            if (versionComparison < 0)
            {
                return LatestReleaseTagName;
            }

            //This local Version and the Version on GitHub are equal.
            else if (versionComparison == 0)
            {
                // If both are pre-release, then compare the pre number.
                if (CurrentVersion.Instance.IsPreRelease && latestPreReleaseNumber is not null
                    && CurrentVersion.Instance.PreReleaseNumber < latestPreReleaseNumber)
                {
                    return LatestReleaseTagName;
                }

                // If local is pre-release, and latest version is not, then GET UPDATE NOW
                else if (CurrentVersion.Instance.IsPreRelease && latestPreReleaseNumber is null)
                {
                    return LatestReleaseTagName;
                }

                else
                {
                    return null;
                }
            }

            //This local version is greater than the release version on GitHub
            else
            {
                return null;
            }
        }
        catch
        {
            Console.WriteLine("I DONOT really care why the fuck it failed but we need to keep running anyway");

            return null;
        }
    }

    private static string PickoutVersionString(in string rawString)
    {
        string pattern = @"\d+\.\d+\.\d+";
        Regex regex = new(pattern);
        return regex.Match(rawString).Value;
    }

    private static int? PickoutPreReleaseNumber(in string rawString)
    {
        string pattern = @"pre\d+";
        Regex regex = new(pattern);
        if (int.TryParse(regex.Match(rawString).Value.Replace("pre", "").AsSpan(), out int result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}
