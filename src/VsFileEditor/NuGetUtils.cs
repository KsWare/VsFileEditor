using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace KsWare.VsFileEditor;

public static class NuGetUtils {

	/// <summary>
	/// Get the latest packet version
	/// </summary>
	/// <param name="packageName">Name of the package</param>
	/// <param name="limitPattern">Optional pattern to limit to specific version. e.g. <c>2.*</c> or  <c>2.1.*</c></param>
	/// <param name="allowPreRelease">[Optional] if specified allow pre-release</param>
	/// <returns>The latest packet version or null</returns>
	/// <remarks>The <paramref name="limitPattern"/> can be simplified. e.g. <c>2</c> will match <c>2.*.*</c></remarks>
	public static string? GetLatestPackageVersion(string packageName, string? limitPattern = null, bool allowPreRelease = false) {
		var limitEx = string.IsNullOrEmpty(limitPattern) ? "" : $"{limitPattern.TrimEnd('*','.')}.";
		var nugetUrl = $"https://api.nuget.org/v3-flatcontainer/{packageName.ToLower()}/index.json";
		try {
			using var client = new HttpClient();
			var response = client.GetStringAsync(nugetUrl).Result;
			var match = Regex.Match(response, @"\""versions\"":\s*\[(.*?)\]", RegexOptions.Singleline);
			if (!match.Success) return null;
			var versions = match.Groups[1].Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(v => NuGetVersion.Parse(v.Trim().Trim('"')))
				.Where(v => v.ToString().StartsWith(limitEx))
				.Where(v => allowPreRelease || !v.IsPrerelease);
			return versions.LastOrDefault()?.ToFullString();
		} catch (Exception ex) {
			Console.Error.WriteLine($"Error fetching the latest version of {packageName}: {ex.Message}");
			return null;
		}
	}

	public static string? GetAvailableVersion(string packageName, string? versionPattern = null) {
		var url = $"https://api.nuget.org/v3-flatcontainer/{packageName.ToLower()}/index.json";
		var response = FetchContentFromUrl(url);
		var versions = Newtonsoft.Json.Linq.JObject.Parse(response)["versions"]
			.Select(v => v.ToString())
			.ToList();

		if (string.IsNullOrEmpty(versionPattern) || !versionPattern.Contains('*'))
			return versions.LastOrDefault();
		return versions.LastOrDefault(v => v.StartsWith(versionPattern.TrimEnd('*')));
	}

	private static string? FetchContentFromUrl(string url) {
		try {
			using var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, url);
			var response = client.Send(request);

			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException("Failed to retrieve data from the provided URL.");
			using var reader = new StreamReader(response.Content.ReadAsStream());
			return reader.ReadToEnd();
		}
		catch (HttpRequestException ex) {
			return null;
		}
	}

}
