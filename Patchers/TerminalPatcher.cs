﻿using HarmonyLib;
using ShipMaid.HelperFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipMaid.Patchers
{
	public static class ShipMaidTerminalCommands
	{
		public static string PerformCleanup()
		{
			ShipMaid.Log("Perform Cleanup Called from terminal");
			LootOrganizingFunctions.OrganizeShipLoot();
			return "Cleaning Ship";
		}
	}

	public static class TerminalExtensions
	{
		/// <summary>
		/// Adds newItem to an array.
		/// </summary>
		/// <typeparam name="T">The type of the array</typeparam>
		/// <param name="array">The array to add to</param>
		/// <param name="newItem">The item to add to the array</param>
		/// <returns></returns>
		public static T[] Add<T>(this T[] array, T newItem)
		{
			int newSize = array.Length + 1;
			Array.Resize(ref array, newSize);
			array[newSize - 1] = newItem;
			return array;
		}

		/// <summary>
		/// Creates a <see cref="TerminalKeyword"/>
		/// </summary>
		/// <param name="word">The terminal command word</param>
		/// <param name="isVerb">Whether the command word is a verb</param>
		/// <param name="triggeringNode">The <see cref="TerminalNode"/> that runs when command word is sent.</param>
		/// <returns>The newly created <see cref="TerminalKeyword"/></returns>
		public static TerminalKeyword CreateTerminalKeyword(string word, bool isVerb = false, TerminalNode triggeringNode = null)
		{
			TerminalKeyword newKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
			newKeyword.word = word.ToLower();
			newKeyword.isVerb = isVerb;
			newKeyword.specialKeywordResult = triggeringNode;
			return newKeyword;
		}

		/// <summary>
		/// Creates a <see cref="TerminalNode"/>
		/// </summary>
		/// <param name="displayText">The text to display on command sent.</param>
		/// <param name="clearPreviousText">Whether to clear the terminal.</param>
		/// <param name="terminalEvent">The terminal you want to trigger. Just keep empty unless you know what you're doing.</param>
		/// <returns></returns>
		public static TerminalNode CreateTerminalNode(string displayText, bool clearPreviousText = false, string terminalEvent = "")
		{
			TerminalNode newNode = ScriptableObject.CreateInstance<TerminalNode>();
			newNode.displayText = displayText;
			newNode.clearPreviousText = clearPreviousText;
			newNode.terminalEvent = terminalEvent;
			return newNode;
		}
	}

	/// <summary>
	/// A harmony patch that runs code after the <see cref="Terminal.TextChanged(string)"/> method on <see cref="global::Terminal"/>
	/// </summary>
	[HarmonyPatch(typeof(Terminal))]
	internal class TerminalPatcher
	{
		public static List<CommandInfo> Commands = new List<CommandInfo>();
		public static Terminal Terminal;

		[HarmonyPatch("Awake")]
		[HarmonyPostfix]
		public static void Awake(ref Terminal __instance)
		{
			Terminal = __instance;

			ShipMaid.Log($"Terminal has awoke");
			TerminalNode triggerNode = TerminalExtensions.CreateTerminalNode("cleanup", true);
			var keyword = TerminalExtensions.CreateTerminalKeyword("cleanup", false, triggerNode);
			Commands.Add(new()
			{
				Title = "cleanup",
				Category = "ShipMaid",
				TriggerNode = triggerNode,
				Description = "Ship Maid Cleanup",
				DisplayTextSupplier = ShipMaidTerminalCommands.PerformCleanup,
			});
			Terminal.terminalNodes.allKeywords = Terminal.terminalNodes.allKeywords.Add(keyword);
			//if (TerminalApi.QueuedDelayedActions.Count > 0)
			//{
			//	TerminalApi.plugin.Log?.LogMessage($"In game, applying any changes now.");
			//	foreach (IDelayedAction delayedAction in TerminalApi.QueuedDelayedActions)
			//	{
			//		delayedAction.Run();
			//	}
			//	TerminalApi.QueuedDelayedActions.Clear();
			//}
			//TerminalAwake?.Invoke((object)__instance, new() { Terminal = __instance });
		}

		[HarmonyPatch("BeginUsingTerminal")]
		[HarmonyPrefix]
		public static void OnBeginUsing(ref Terminal __instance)
		{
		}

		[HarmonyPatch("QuitTerminal")]
		[HarmonyPostfix]
		public static void OnQuitTerminal(ref Terminal __instance)
		{
		}

		[HarmonyPatch("TextChanged")]
		[HarmonyPostfix]
		public static void OnTextChanged(ref Terminal __instance, string newText)
		{
			string currentInputText = "";
			if (newText.Trim().Length >= __instance.textAdded)
			{
				currentInputText = newText.Substring(newText.Length - __instance.textAdded);
			}

			ShipMaid.Log($"{currentInputText}");
		}

		[HarmonyPatch("ParsePlayerSentence")]
		[HarmonyPostfix]
		public static void ParsePlayerSentence(ref Terminal __instance, TerminalNode __result)
		{
			CommandInfo commandInfo = Commands.FirstOrDefault(cI => cI.TriggerNode == __result);

			// Calls callback function, if there is one
			if (commandInfo != null)
			{
				__result.displayText = commandInfo?.DisplayTextSupplier();
			}

			string submittedText = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
			ShipMaid.Log($"Submitted Text - {submittedText}");
		}
	}
}