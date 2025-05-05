namespace MauiTestFile
{
    //! @todo FilePickerの参考サイト
    //! https://learn.microsoft.com/ja-jp/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-8.0&tabs=android

    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ファイル選択ボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント情報</param>
        private async void OnFileSelectBtnClicked(object sender, EventArgs e)
        {
            try
            {
                // ファイル選択ダイアログのオプションを設定
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "json" } },                   // iOS UTType を使うらしい(iPhone持ってないので未検証)
                        { DevicePlatform.Android, new[] { "application/json" } },   // Android MIME type
                        { DevicePlatform.WinUI, new[] { ".json", ".json" } },       // Windows 拡張子
                        { DevicePlatform.macOS, new[] { "public.json" } },          // macOS UTType(MAC持ってないので未検証)
                        { DevicePlatform.Tizen, new[] { "application/json" } },     // Tizen MIME type・・・使ってる人いるの？
                    });

                PickOptions options = new()
                {
                    PickerTitle = "JSONファイルを選択してください",
                    FileTypes = customFileType,
                };

                // ファイル選択ダイアログを表示
                //var result = await FilePicker.Default.PickAsync(options);

                // 複数ファイル選択ダイアログを表示
                var results = await FilePicker.Default.PickMultipleAsync(options);

                if (results != null && results.Any())
                {
                    // "json" フォルダを作成
                    string jsonFolderPath = Path.Combine(FileSystem.AppDataDirectory, "json");
                    Directory.CreateDirectory(jsonFolderPath);

                    foreach (var file in results)
                    {
                        try
                        {
                            // ファイルの内容を読み取る
                            string fileContent = await File.ReadAllTextAsync(file.FullPath);

                            // JSON をパースして整合性チェック
                            var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object>(fileContent);

                            // ファイル名を保持して保存
                            string localFilePath = Path.Combine(jsonFolderPath, file.FileName);
                            await File.WriteAllTextAsync(localFilePath, fileContent);
                        }
                        catch (Exception jsonEx)
                        {
                            // JSON の解析エラーを通知
                            await DisplayAlert("JSON エラー", $"ファイル {file.FileName} の解析中にエラーが発生しました: {jsonEx.Message}", "OK");
                        }
                    }

                    // 保存完了メッセージを表示
                    await DisplayAlert("保存完了", $"選択されたファイルが保存されました: {jsonFolderPath}", "OK");
                }
                else
                {
                    // ファイルが選択されなかった場合
                    await DisplayAlert("ファイル選択", "ファイルが選択されませんでした。", "OK");
                }
            }
            catch (Exception ex)
            {
                // エラー処理
                await DisplayAlert("エラー", $"エラーが発生しました: {ex.Message}", "OK");
            }
        }
    }

}
