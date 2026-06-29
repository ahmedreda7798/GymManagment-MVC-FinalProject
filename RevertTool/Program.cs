using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RevertTool
{
    class Program
    {
        class EditOperation
        {
            public string TargetFile { get; set; } = "";
            public string OldContent { get; set; } = "";
            public string NewContent { get; set; } = "";
        }

        static void Main(string[] args)
        {
            string transcriptPath = @"C:\Users\ahmed\.gemini\antigravity-ide\brain\eca8a043-84cb-4e30-8443-3400338eeb03\.system_generated\logs\transcript_full.jsonl";
            var lines = File.ReadAllLines(transcriptPath);
            
            var allEdits = new List<EditOperation>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                try 
                {
                    using var doc = JsonDocument.Parse(line);
                    var root = doc.RootElement;
                    
                    if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "PLANNER_RESPONSE")
                    {
                        if (root.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var toolCall in toolCalls.EnumerateArray())
                            {
                                var name = toolCall.GetProperty("name").GetString();
                                if (name == "default_api:replace_file_content")
                                {
                                    var args = toolCall.GetProperty("arguments");
                                    var targetFile = args.GetProperty("TargetFile").GetString();
                                    var oldContent = args.GetProperty("TargetContent").GetString();
                                    var newContent = args.GetProperty("ReplacementContent").GetString();
                                    if(targetFile != null && oldContent != null && newContent != null)
                                        allEdits.Add(new EditOperation { TargetFile = targetFile, OldContent = oldContent, NewContent = newContent });
                                }
                                else if (name == "default_api:multi_replace_file_content")
                                {
                                    var toolArgs = toolCall.GetProperty("arguments");
                                    var targetFile = toolArgs.GetProperty("TargetFile").GetString();
                                    if(targetFile == null) continue;

                                    var chunks = toolArgs.GetProperty("ReplacementChunks");
                                    foreach (var chunk in chunks.EnumerateArray())
                                    {
                                        var oldContent = chunk.GetProperty("TargetContent").GetString();
                                        var newContent = chunk.GetProperty("ReplacementContent").GetString();
                                        if(oldContent != null && newContent != null)
                                            allEdits.Add(new EditOperation { TargetFile = targetFile, OldContent = oldContent, NewContent = newContent });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing line: " + ex.Message);
                }
            }

            Console.WriteLine($"Found {allEdits.Count} edit operations to revert.");

            allEdits.Reverse();

            int successCount = 0;
            foreach (var edit in allEdits)
            {
                if (File.Exists(edit.TargetFile))
                {
                    try 
                    {
                        string content = File.ReadAllText(edit.TargetFile);
                        string normalizedContent = content.Replace("\r\n", "\n");
                        string normalizedNewContent = edit.NewContent.Replace("\r\n", "\n");

                        if (normalizedContent.Contains(normalizedNewContent))
                        {
                            normalizedContent = normalizedContent.Replace(normalizedNewContent, edit.OldContent.Replace("\r\n", "\n"));
                            File.WriteAllText(edit.TargetFile, normalizedContent);
                            Console.WriteLine($"Reverted changes in {edit.TargetFile}");
                            successCount++;
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Could not find NewContent in {edit.TargetFile} to revert.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading/writing {edit.TargetFile}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"Successfully reverted {successCount} out of {allEdits.Count} edits.");
        }
    }
}
