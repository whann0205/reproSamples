using Microsoft.Extensions.Logging;
using FailedDBAccess.Data;

using SQLite;

using Microsoft.Maui.Controls;

namespace FailedDBAccess;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var dbkey = "1234";
        var dbfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        var dblivefile = Path.Combine(dbfolder, "AssetQueryLive.db");
        var dbrelfile = Path.Combine(dbfolder, "AssetQuery.db");
		SQLiteAsyncConnection CurrentDB;

        static async Task InstallDB(string dbpath)
		{
			string templateFileName;

			templateFileName = "dummy.db";

			var mainDir = FileSystem.AppDataDirectory;

			using (var stream_source = await FileSystem.OpenAppPackageFileAsync(templateFileName))
			{
				using (var outputFileStream = new FileStream(dbpath, FileMode.Create))
				{
					await stream_source.CopyToAsync(outputFileStream);

				}

			}
		}

	    async static Task <string> GetDummyAsync(SQLiteAsyncConnection CurrentDB)
        {

          
            int dummy = 0;
            string error="";

            try
            {
                string query = $@"select dummy from dummy ";

                dummy = await CurrentDB
                    .ExecuteScalarAsync<int>(query);
            }
            catch (SQLiteException sqlEx)

            {
                error = sqlEx.GetBaseException().Message;
            }
            return "Return value "+ dummy + " Error: "+error;

        }


        var Itask = Task.Run(() => InstallDB(dbrelfile));
        Itask.Wait();

        var optionsc = new SQLiteConnectionString(dbrelfile, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true, key: dbkey);
        CurrentDB = new SQLiteAsyncConnection(optionsc);

        var Dtask = Task.Run(() => GetDummyAsync(CurrentDB));
        Dtask.Wait();

        var dummy = Dtask.Result;

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
