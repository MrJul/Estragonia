﻿using System;
using System.Threading.Tasks;
using Godot;
using GdArray = Godot.Collections.Array;

namespace GameMenu.UI;

public static class AsyncGodotResourceLoader {

	public static Task<T> LoadAsync<T>(
		string path,
		ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse,
		IProgress<double>? progress = null
	)
	where T : Resource {
		var tcs = new TaskCompletionSource<T>();

		var error = ResourceLoader.LoadThreadedRequest(path, typeHint: typeof(T).Name, cacheMode: cacheMode);
		if (error != Error.Ok) {
			tcs.TrySetException(new InvalidOperationException($"Failed to start loading resource {path}: {error}"));
			return tcs.Task;
		}

		var progressHolder = new GdArray();

		Action? updateStatus = null;

		updateStatus = () => {
			var status = ResourceLoader.LoadThreadedGetStatus(path, progressHolder);

			switch (status) {
				case ResourceLoader.ThreadLoadStatus.InvalidResource:
					tcs.TrySetException(new InvalidOperationException($"Invalid resource {path}"));
					break;

				case ResourceLoader.ThreadLoadStatus.InProgress:
					progress?.Report(progressHolder[0].As<double>());
					Task.Delay(TimeSpan.FromMilliseconds(50))
						.ContinueWith(
							(_, state) => ((Action) state!).Invoke(),
							updateStatus,
							TaskScheduler.FromCurrentSynchronizationContext()
						);
					break;

				case ResourceLoader.ThreadLoadStatus.Failed:
					tcs.TrySetException(new InvalidOperationException($"Failed to load resource {path}"));
					break;

				case ResourceLoader.ThreadLoadStatus.Loaded:
					progress?.Report(1.0);
					tcs.TrySetResult((T) ResourceLoader.LoadThreadedGet(path));
					break;

				default:
					throw new InvalidOperationException($"Unexpected status {status}");
			}
		};

		updateStatus();

		return tcs.Task;
	}

}
