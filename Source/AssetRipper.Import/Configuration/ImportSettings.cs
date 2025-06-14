﻿using AssetRipper.Import.Logging;
using System.Text.Json.Serialization;

namespace AssetRipper.Import.Configuration;

public sealed record class ImportSettings
{
	/// <summary>
	/// The level of scripts to export
	/// </summary>
	public ScriptContentLevel ScriptContentLevel { get; set; } = ScriptContentLevel.Level2;

	/// <summary>
	/// Including the streaming assets directory can cause some games to fail while exporting.
	/// </summary>
	[JsonIgnore]
	public bool IgnoreStreamingAssets
	{
		get => StreamingAssetsMode == StreamingAssetsMode.Ignore;
		set
		{
			StreamingAssetsMode = value ? StreamingAssetsMode.Ignore : StreamingAssetsMode.Extract;
		}
	}

	/// <summary>
	/// How the StreamingAssets folder is handled
	/// </summary>
	public StreamingAssetsMode StreamingAssetsMode { get; set; } = StreamingAssetsMode.Extract;

	/// <summary>
	/// The default version used when no version is specified, ie when the version has been stripped.
	/// </summary>
	[JsonConverter(typeof(UnityVersionJsonConverter))]
	public UnityVersion DefaultVersion { get; set; }

	/// <summary>
	/// The target version to convert all assets to. Experimental
	/// </summary>
        [JsonConverter(typeof(UnityVersionJsonConverter))]
        public UnityVersion TargetVersion { get; set; }

        /// <summary>
        /// API key for OpenAI GPT models used in script level 4 recovery.
        /// Stored in plain text. Keep this safe.
        /// </summary>
        public string? OpenAIApiKey { get; set; }

        /// <summary>
        /// API key for Anthropic Claude models used in script level 4 recovery.
        /// Stored in plain text. Keep this safe.
        /// </summary>
        public string? AnthropicApiKey { get; set; }

	public void Log()
	{
		Logger.Info(LogCategory.General, $"{nameof(ScriptContentLevel)}: {ScriptContentLevel}");
		Logger.Info(LogCategory.General, $"{nameof(StreamingAssetsMode)}: {StreamingAssetsMode}");
                Logger.Info(LogCategory.General, $"{nameof(DefaultVersion)}: {DefaultVersion}");
                Logger.Info(LogCategory.General, $"{nameof(TargetVersion)}: {TargetVersion}");
                Logger.Info(LogCategory.General, $"OpenAI Key Provided: {!string.IsNullOrEmpty(OpenAIApiKey)}");
                Logger.Info(LogCategory.General, $"Anthropic Key Provided: {!string.IsNullOrEmpty(AnthropicApiKey)}");
        }
}
