using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Godot;
using Environment = System.Environment;

namespace JLeb.Estragonia;

/// <summary>Implementation of <see cref="IStorageProvider"/> for Godot.</summary>
internal sealed class GodotStorageProvider : IStorageProvider {

	public bool CanOpen
		=> true;

	public bool CanSave
		=> true;

	public bool CanPickFolder
		=> true;

	public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
		=> PickFilesAsync(
			options,
			options.AllowMultiple ? FileDialog.FileModeEnum.OpenFiles : FileDialog.FileModeEnum.OpenFile,
			options.FileTypeFilter
		);

	public async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options) {
		var files = await PickFilesAsync(options, FileDialog.FileModeEnum.SaveFile, options.FileTypeChoices);
		return files.Count > 0 ? files[0] : null;
	}

	public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options) {
		var folders = Array.Empty<IStorageFolder>();
		var dialog = CreateDialog(options, FileDialog.FileModeEnum.OpenDir);

		dialog.DirSelected += OnDirSelected;

		void OnDirSelected(string dir) {
			dialog.DirSelected -= OnDirSelected;
			folders = new IStorageFolder[] { new BclStorageFolder(new DirectoryInfo(dir)) };
		}

		dialog.Show();

		return Task.FromResult<IReadOnlyList<IStorageFolder>>(folders);
	}

	public Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark) {
		var fileInfo = new FileInfo(bookmark);
		var storageFile = fileInfo.Exists ? new BclStorageFile(fileInfo) : null;
		return Task.FromResult<IStorageBookmarkFile?>(storageFile);
	}

	public Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark) {
		var folderInfo = new DirectoryInfo(bookmark);
		var storageFolder = folderInfo.Exists ? new BclStorageFolder(folderInfo) : null;
		return Task.FromResult<IStorageBookmarkFolder?>(storageFolder);
	}

	public Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath) {
		if (filePath.IsAbsoluteUri) {
			var fileInfo = new FileInfo(filePath.LocalPath);
			if (fileInfo.Exists)
				return Task.FromResult<IStorageFile?>(new BclStorageFile(fileInfo));
		}

		return Task.FromResult<IStorageFile?>(null);
	}

	public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath) {
		if (folderPath.IsAbsoluteUri) {
			var folderInfo = new DirectoryInfo(folderPath.LocalPath);
			if (folderInfo.Exists)
				return Task.FromResult<IStorageFolder?>(new BclStorageFolder(folderInfo));
		}

		return Task.FromResult<IStorageFolder?>(null);
	}

	public Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder) {
		var path = wellKnownFolder switch {
			WellKnownFolder.Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
			WellKnownFolder.Documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			WellKnownFolder.Music => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
			WellKnownFolder.Pictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
			WellKnownFolder.Videos => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
			_ => null
		};

		var storageFolder = path is null ? null : new BclStorageFolder(new DirectoryInfo(path));
		return Task.FromResult<IStorageFolder?>(storageFolder);
	}

	private static Task<IReadOnlyList<IStorageFile>> PickFilesAsync(
		PickerOptions options,
		FileDialog.FileModeEnum fileMode,
		IReadOnlyList<FilePickerFileType>? fileTypes
	) {
		var dialog = CreateDialog(options, fileMode);

		if (fileTypes is not null) {
			foreach (var fileType in fileTypes)
				dialog.AddFilter(String.Join(',', fileType.Patterns ?? Array.Empty<string>()), fileType.Name);
		}

		if (fileMode == FileDialog.FileModeEnum.OpenFiles)
			dialog.FilesSelected += OnFilesSelected;
		else
			dialog.FileSelected += OnFileSelected;

		var storageFiles = Array.Empty<BclStorageFile>();

		void OnFilesSelected(string[] paths) {
			dialog.FilesSelected -= OnFilesSelected;
			storageFiles = paths.Select(path => new BclStorageFile(new FileInfo(path))).ToArray();
		}

		void OnFileSelected(string path) {
			dialog.FileSelected -= OnFileSelected;
			storageFiles = new[] { new BclStorageFile(new FileInfo(path)) };
		}

		dialog.Show();

		return Task.FromResult<IReadOnlyList<IStorageFile>>(storageFiles);
	}

	private static FileDialog CreateDialog(PickerOptions options, FileDialog.FileModeEnum fileMode)
		=> new() {
			Access = FileDialog.AccessEnum.Filesystem,
			CurrentDir = options.SuggestedStartLocation?.TryGetLocalPath(),
			Exclusive = true,
			FileMode = fileMode,
			ModeOverridesTitle = String.IsNullOrEmpty(options.Title),
			Title = options.Title,
			Transient = true,
			UseNativeDialog = true
		};

}
