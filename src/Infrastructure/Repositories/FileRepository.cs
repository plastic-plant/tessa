﻿using Microsoft.Extensions.Logging;
using Tessa.Application.Enums;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;

namespace Tessa.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
	private readonly ILogger<FileRepository> _logger;

    public FileRepository(ILogger<FileRepository> logger)
    {
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public PathSummary GetPathSummary(string givenPath)
	{
		var summary = new PathSummary()
		{
			PathOriginal = givenPath,
			PathRooted = Path.IsPathRooted(givenPath) ? givenPath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, givenPath)
		};

		summary.IsExistingDirectory = Directory.Exists(summary.PathRooted);
		summary.IsExistingFile = File.Exists(summary.PathRooted);
		return summary;
	}

	/// <summary>
	/// Gets a summary of files in given path. Path can be a directory or a single file.
	/// By default orders alphabatically and searches subdirectories.
	/// </summary>
	public List<FileSummary> GetFilesSummary(string path, SearchOption search = SearchOption.AllDirectories, FileOrder order = FileOrder.Alphabetical)
	{
		var list = new List<FileSummary>();			

		var summary = GetPathSummary(path);
		if (summary.IsExistingDirectory)
		{
			_logger.LogDebug($"Input path is a directory: {summary.PathRooted}");			
			string[] filenames = Directory.GetFiles(summary.PathRooted!, "*.*", search);
			var ordered = order switch
			{
				FileOrder.Alphabetical => filenames.OrderBy(filename => filename).ToList(),
				FileOrder.ReverseAlphabetical => filenames.OrderByDescending(filename => filename).ToList(),
				FileOrder.CreationDate => filenames.OrderBy(filename => File.GetCreationTime(filename)).ToList(),
				FileOrder.ReverseCreationDate => filenames.OrderByDescending(filename => File.GetCreationTime(filename)).ToList(),
				FileOrder.ModifiedDate => filenames.OrderBy(filename => File.GetLastWriteTime(filename)).ToList(),
				FileOrder.ReverseModifiedDate => filenames.OrderByDescending(filename => File.GetLastWriteTime(filename)).ToList(),
				FileOrder.None => filenames.ToList(),
				_ => filenames.ToList()
			};

			ordered.ForEach(filename => list.Add(FileSummary.From(filename)));
		}
		else
		{
			_logger.LogDebug($"Input path is a single file: {summary.PathRooted}");
			list.Add(FileSummary.From(summary.PathRooted!));
		}

		return list;
	}
}
