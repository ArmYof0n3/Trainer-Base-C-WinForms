﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheatEngine;

namespace Cheats
{
	public class myCheats
	{
		public CE LibMem = new CE();

		public bool openProc;
		public long[] AoBScan = new long[8];
		public long[] CodeCave = new long[7];
		public long minRange = 0;
		public long maxRange = 0;
		public string ProcName = null;
		public int pID = 0;
		public bool StartPID = false;
		public bool RangePID = false;

		public bool UWP = true;

		public int TimerChange = 0;

		public string Waypoint = "0F 57 F6 F3 0F 5F C6 F3 0F 11 47";
		public string Velocity = "F3 44 0F * * * 41 0F 28 C1 44 0F * * * * F3 0F";
		public string Timer = "F3 0F 5C C1 F3 0F 11 * * F3 0F 11 * * * * * 48";
		public string FreezeAI = "F3 0F 10 * * * * * 49 8B F8 48 * * * 48 8B";
		public string MoneyPtr = "48 89 78 * 4C 8B F1 4C 8D * * * * * 49";
		public string Money = "48 40 53 57 48 83 EC 38 48 * * * * * * * * * * * E8 * * * * 48 8B D8 48 * * * * 48 * * * * * 48 8B C8 FF * * * * * * 0F * * * * * * * * * 48 8D 4C 24 50 38";
		public string MoneyUwp = "40 55 53 40 53 57 48 83 EC 38 48 * * * * * * * * 48 8B F9 E8 * * * * 48 8B D8 48 * * * * 48 * * * * * 48 8B C8 FF * * * * * * 0F * * * 41 * * * * * 48 8D 4C 24 50";
		public string WheelSpins = "CC * 40 53 57 48 83 EC 38 48 * * * * * * * * * * * E8 * * * * 48 8B D8 48 * * * * 48 * * * * * 48 8B C8 FF * * * * * * 0F * * * * * * * * * 48 8D 4C 24 50 38";
		public string WheelSpinsUwp = "CC CC 40 53 57 48 83 EC 38 48 * * * * * * * * * * * E8 * * * * 48 8B D8 48 * * * * 48 * * * * * 48 8B C8 FF * * * * * * 0F * * * * * * * * * 48 8D 4C 24 50 38";
		public string PerkPoints = "BC * * * * 53 57 48 83 EC 38 48 * * * * * * * * 48 * * * E8 * * * * 48 8B D8 48 * * * * 48 * * * 74 * 48 8B C8 FF * * * * * * 0F * * * * * * 41 * * * * * 48 8D 4C 24 50";
		public string PerkPointsUwp = "1E 40 53 57 48 83 EC 38 48 * * * * * * * * 48 * * * E8 * * * * 48 8B D8 48 * * * * 48 * * * 74 * 48 8B C8 FF * * * * * * 0F * * * * * * 41 * * * * * 48 8D 4C 24 50";

		public void TrainerClose()
		{

			if (CodeCave[0] > 0)
			{
				if(CodeCave[2] > 0)
					LibMem.WriteMemory(AoBScan[3].ToString("X"), "bytes", "F3 0F 10 89 AC 06 00 00"); // Freeze AI

				if (CodeCave[1] > 0)
					LibMem.WriteMemory(AoBScan[2].ToString("X"), "bytes", "F3 44 0F 10 49 24"); // Velocity

				LibMem.WriteMemory(AoBScan[1].ToString("X"), "bytes", "0F 57 F6 F3 0F 5F C6"); // Waypoint
			}

			if (CodeCave[4] > 0)
			{
				if (CodeCave[3] > 0)
					LibMem.WriteMemory(AoBScan[4].ToString("X"), "bytes", "48 8D 4C 24 50"); // Money

				if (CodeCave[5] > 0)
					LibMem.WriteMemory(AoBScan[6].ToString("X"), "bytes", "48 8D 4C 24 50"); // Wheelspin

				if (CodeCave[6] > 0)
					LibMem.WriteMemory(AoBScan[7].ToString("X"), "bytes", "48 8D 4C 24 50"); // PerkPoints

				LibMem.WriteMemory(AoBScan[5].ToString("X"), "bytes", "48 89 78 20 4C 8B F1"); // MoneyPtr
			}

			if (AoBScan[0] > 0)
				LibMem.WriteMemory(AoBScan[0].ToString("X"), "bytes", "F3 0F 5C C1"); // Timer


			for (int i = 0; i < CodeCave.Length; i++)
			{
				if (CodeCave[i] > 0)
				{
					LibMem.VirtualFreeEX(LibMem.pHandle, (UIntPtr)CodeCave[i], (UIntPtr)0, 0x8000);
				}
			}
		}

		public void OpenGame()
		{
			string[] Multiple = new string[2]
			{
				"ForzaHorizon4",
				"Microsoft.SunriseBaseGame_1.467.173.2_x64__8wekyb3d8bbwe"
			};

			for (int i = 0; i < Multiple.Length; i++)
			{
				pID = LibMem.GetProcIdFromName(Multiple[i]);

				if (pID > 0)
					break;
			}
			
			if (pID > 0)
			{
				if (!StartPID)
				{
					StartPID = true;
					Thread.Sleep(2500);
				}

				openProc = LibMem.OpenProcess(pID);

				if (openProc && !RangePID)
				{

					minRange = (long)LibMem.theProc.MainModule.BaseAddress;
					maxRange = (long)LibMem.theProc.MainModule.BaseAddress + LibMem.theProc.MainModule.ModuleMemorySize;
					ProcName = LibMem.theProc.ProcessName + ".exe - UWP";

					if (minRange > 0 && maxRange > 0)
						RangePID = true;

					foreach (var mod in LibMem.modules)
					{
						if (mod.Key == "steam_api64.dll")
						{
							UWP = false;
							ProcName = LibMem.theProc.ProcessName + ".exe - STEAM";
						}
					}
				}
			}
			else
			{

				for (int i = 0; i < AoBScan.Length; i++) // Check and reset AobScan
					if (AoBScan[i] != 0) AoBScan[i] = 0;

				for (int i = 0; i < CodeCave.Length; i++) // Check and reset CodeCave
					if (CodeCave[i] != 0) CodeCave[i] = 0;


				pID = 0;
				openProc = false;
				StartPID = false;
				RangePID = false;
				ProcName = null;
				minRange = 0;
				maxRange = 0;

				TimerChange = 0;
				UWP = true;
			}
		}

		public async Task CheatMoneyPtr()
		{
			AoBScan[5] = 0; CodeCave[4] = 0;

			AoBScan[5] = (await LibMem.AoBScan(minRange, maxRange, MoneyPtr, false, false, true)).FirstOrDefault();

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[5] > 0)
			{
				byte[] asm = { 0x56, 0x48, 0x8D, 0x70, 0x20, 0x48, 0x89, 0x35, 0x1B, 0x00, 0x00, 0x00, 0x48, 0x8D, 0xB6, 0xA1, 0xFC, 0xFF, 0xFF, 0x48, 0x89, 0x35, 0x15, 0x00, 0x00, 0x00, 0x5E, 0x48, 0x89, 0x78, 0x20, 0x4C, 0x8B, 0xF1 };

				CodeCave[4] = (long)LibMem.CreateCodeCave(AoBScan[5].ToString("X"), asm, 7);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + MoneyPtr + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatMoney()
		{
			AoBScan[4] = 0; CodeCave[3] = 0;

			AoBScan[4] = (await LibMem.AoBScan(minRange, maxRange, Money, false, false, true)).FirstOrDefault() + 0x3B;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[4] > 0x3B)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x7D, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x27, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x4C, 0x39, 0xF0, 0x58, 0x0F, 0x85, 0x68, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x01, 0x0F, 0x84, 0x0F, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x00, 0x0F, 0x84, 0x29, 0x00, 0x00, 0x00, 0xE9, 0x4F, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x08, 0x00, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x0C, 0x42, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x2B, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x21, 0x99, 0x62, 0xC7, 0x47, 0x08, 0x00, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x08, 0x01, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x0C, 0x42, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[3] = (long)LibMem.CreateCodeCave(AoBScan[4].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + Money + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatMoneyUwp()
		{
			AoBScan[4] = 0; CodeCave[3] = 0;

			AoBScan[4] = (await LibMem.AoBScan(minRange, maxRange, MoneyUwp, false, false, true)).FirstOrDefault() + 0x3D;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[4] > 0x3D)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x91, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x81, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x27, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x8D, 0x40, 0x10, 0x4C, 0x39, 0xF0, 0x58, 0x0F, 0x85, 0x68, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x01, 0x0F, 0x84, 0x0F, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x00, 0x0F, 0x84, 0x29, 0x00, 0x00, 0x00, 0xE9, 0x4F, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x08, 0x00, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x0C, 0x42, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x2B, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x21, 0x99, 0x62, 0xC7, 0x47, 0x08, 0x00, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x08, 0x01, 0x0C, 0xBD, 0x38, 0xC7, 0x47, 0x0C, 0x42, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[3] = (long)LibMem.CreateCodeCave(AoBScan[4].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + MoneyUwp + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatWheelSpins()
		{
			AoBScan[6] = 0; CodeCave[5] = 0;

			AoBScan[6] = (await LibMem.AoBScan(minRange, maxRange, WheelSpins, false, false, true)).FirstOrDefault() + 0x3C;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[6] > 0x3C)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x73, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x63, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x2F, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x39, 0xE8, 0x58, 0x0F, 0x85, 0x4E, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x00, 0x0F, 0x84, 0x0A, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x01, 0x0F, 0x84, 0x1D, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x00, 0x1A, 0x77, 0xA2, 0xC7, 0x47, 0x08, 0x01, 0x1A, 0x77, 0xA2, 0xC7, 0x05, 0x2C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x1D, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x1A, 0x77, 0xA2, 0xC7, 0x47, 0x08, 0x00, 0x1A, 0x77, 0xA2, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[5] = (long)LibMem.CreateCodeCave(AoBScan[6].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + WheelSpins + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatWheelSpinsUwp()
		{
			AoBScan[6] = 0; CodeCave[5] = 0;

			AoBScan[6] = (await LibMem.AoBScan(minRange, maxRange, WheelSpinsUwp, false, false, true)).FirstOrDefault() + 0x3C;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[6] > 0x3C)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x77, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x67, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x2F, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x8D, 0x40, 0x10, 0x48, 0x39, 0xE8, 0x58, 0x0F, 0x85, 0x4E, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x00, 0x0F, 0x84, 0x0A, 0x00, 0x00, 0x00, 0x80, 0x7F, 0x08, 0x01, 0x0F, 0x84, 0x1D, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x00, 0x59, 0x9A, 0x74, 0xC7, 0x47, 0x08, 0x01, 0x59, 0x9A, 0x74, 0xC7, 0x05, 0x2C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x1D, 0x00, 0x00, 0x00, 0xC7, 0x47, 0x08, 0x01, 0x59, 0x9A, 0x74, 0xC7, 0x47, 0x08, 0x00, 0x59, 0x9A, 0x74, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[5] = (long)LibMem.CreateCodeCave(AoBScan[6].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + WheelSpinsUwp + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatPerkPoints()
		{
			AoBScan[7] = 0; CodeCave[6] = 0;

			AoBScan[7] = (await LibMem.AoBScan(minRange, maxRange, PerkPoints, false, false, true)).FirstOrDefault() + 0x42;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[7] > 0x42)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x91, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x81, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x27, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x8D, 0x80, 0xE8, 0xFA, 0xFF, 0xFF, 0x48, 0x39, 0xE8, 0x58, 0x0F, 0x85, 0x65, 0x00, 0x00, 0x00, 0x80, 0xBF, 0x88, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x12, 0x00, 0x00, 0x00, 0x80, 0xBF, 0x88, 0x00, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x28, 0x00, 0x00, 0x00, 0xE9, 0x46, 0x00, 0x00, 0x00, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x00, 0x14, 0x77, 0xA2, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x01, 0x14, 0x77, 0xA2, 0xC7, 0x05, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x23, 0x00, 0x00, 0x00, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x01, 0x14, 0x77, 0xA2, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x00, 0x14, 0x77, 0xA2, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[6] = (long)LibMem.CreateCodeCave(AoBScan[7].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + PerkPoints + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatPerkPointsUwp()
		{
			AoBScan[7] = 0; CodeCave[6] = 0;

			AoBScan[7] = (await LibMem.AoBScan(minRange, maxRange, PerkPointsUwp, false, false, true)).FirstOrDefault() + 0x3F;

			//MessageBox.Show(AoBScan[4].ToString("X"));

			if (AoBScan[7] > 0x3F)
			{
				if (CodeCave[4] == 0)
					await CheatMoneyPtr();

				if (CodeCave[4] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[4]);

				byte[] asm = { 0x83, 0x3D, 0x91, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x81, 0x00, 0x00, 0x00, 0x50, 0x48, 0xA1, 0x27, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x8D, 0x80, 0xF8, 0xFA, 0xFF, 0xFF, 0x48, 0x39, 0xE8, 0x58, 0x0F, 0x85, 0x65, 0x00, 0x00, 0x00, 0x80, 0xBF, 0x88, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x12, 0x00, 0x00, 0x00, 0x80, 0xBF, 0x88, 0x00, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x28, 0x00, 0x00, 0x00, 0xE9, 0x46, 0x00, 0x00, 0x00, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x00, 0x59, 0x9A, 0x74, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x01, 0x59, 0x9A, 0x74, 0xC7, 0x05, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x23, 0x00, 0x00, 0x00, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x01, 0x59, 0x9A, 0x74, 0xC7, 0x87, 0x88, 0x00, 0x00, 0x00, 0x00, 0x59, 0x9A, 0x74, 0xC7, 0x05, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x50 };

				CodeCave[6] = (long)LibMem.CreateCodeCave(AoBScan[7].ToString("X"), asm, 5);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + PerkPointsUwp + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatTimer()
		{
			if (TimerChange == 0)
			{
				AoBScan[0] = 0;

				AoBScan[0] = (await LibMem.AoBScan(minRange, maxRange, Timer, false, false, true)).FirstOrDefault();

				if (AoBScan[0] > 0)
				{
					LibMem.WriteMemory(AoBScan[0].ToString("X"), "bytes", "90 90 90 90");

					TimerChange = 1;
				}
				else
					MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + Timer + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);

			}
			else if (TimerChange == 1)
            {
				LibMem.WriteMemory(AoBScan[0].ToString("X"), "bytes", "F3 0F 5C C1");

				TimerChange = 2;
			}
			else if (TimerChange == 2)
			{
				LibMem.WriteMemory(AoBScan[0].ToString("X"), "bytes", "90 90 90 90");

				TimerChange = 1;
			}


		}

		public async Task CheatWaypoint()
		{			
			AoBScan[1] = 0; CodeCave[0] = 0;
				
			AoBScan[1] = (await LibMem.AoBScan(minRange, maxRange, Waypoint, false, false, true)).FirstOrDefault();

			if (AoBScan[1] > 0)
			{
				byte[] asm = { 0x56, 0x83, 0x7F, 0x40, 0x00, 0x0F, 0x84, 0x2B, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x77, 0x30, 0xF3, 0x0F, 0x10, 0x46, 0x14, 0xF3, 0x0F, 0x11, 0x05, 0x27, 0x00, 0x00, 0x00, 0xF3, 0x0F, 0x10, 0x46, 0x18, 0xF3, 0x0F, 0x11, 0x05, 0x1E, 0x00, 0x00, 0x00, 0xF3, 0x0F, 0x10, 0x46, 0x1C, 0xF3, 0x0F, 0x11, 0x05, 0x15, 0x00, 0x00, 0x00, 0x5E, 0x0F, 0x57, 0xF6, 0xF3, 0x0F, 0x5F, 0xC6 };

				CodeCave[0] = (long)LibMem.CreateCodeCave(AoBScan[1].ToString("X"), asm, 7);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + Waypoint + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatVelocity()
		{
			AoBScan[2] = 0; CodeCave[1] = 0;

			AoBScan[2] = (await LibMem.AoBScan(minRange, maxRange, Velocity, false, false, true)).FirstOrDefault();

			if (AoBScan[2] > 0)
			{
				if (CodeCave[0] == 0)
					await CheatWaypoint();

				if (CodeCave[0] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[0]);

				byte[] asm = {0x48, 0x89, 0x0D, 0x96, 0x02, 0x00, 0x00, 0x83, 0x3D, 0x5B, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x53, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x52, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x88, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x49, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x9F, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x58, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0xB3, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x4F, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0xE2, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x46, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0x4B, 0x01, 0x00, 0x00, 0x83, 0x3D, 0x3D, 0x02, 0x00, 0x00, 0x01, 0x0F, 0x84, 0xB3, 0x01, 0x00, 0x00, 0xE9, 0xF7, 0x01, 0x00, 0x00, 0xC7, 0x05, 0xF8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x20, 0xF3, 0x45, 0x0F, 0x58, 0xC9, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x20, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x24, 0xF3, 0x45, 0x0F, 0x58, 0xC9, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x24, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x28, 0xF3, 0x45, 0x0F, 0x58, 0xC9, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x28, 0xE9, 0xB5, 0x01, 0x00, 0x00, 0xC7, 0x05, 0xBA, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC7, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00, 0xC7, 0x41, 0x24, 0x00, 0x00, 0x00, 0x00, 0xC7, 0x41, 0x28, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x91, 0x01, 0x00, 0x00, 0xC7, 0x05, 0x9A, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x68, 0x00, 0x00, 0x20, 0x41, 0xD9, 0x41, 0x24, 0xD8, 0x04, 0x24, 0xD9, 0x59, 0x24, 0x48, 0x83, 0xC4, 0x08, 0xE9, 0x70, 0x01, 0x00, 0x00, 0xC7, 0x05, 0x95, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x40, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x6E, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x44, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x63, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x48, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x58, 0x01, 0x00, 0x00, 0xE9, 0x34, 0x01, 0x00, 0x00, 0xC7, 0x05, 0x5D, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x3A, 0x01, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x1D, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x40, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x31, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x44, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x26, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x48, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0x1B, 0x01, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0xFE, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x40, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0xF3, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x44, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0xE8, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x48, 0xE9, 0xBE, 0x00, 0x00, 0x00, 0xC7, 0x05, 0xEB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x92, 0xFE, 0x00, 0x00, 0x00, 0x0F, 0x84, 0xA7, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x40, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0xBB, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x44, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0xB0, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x48, 0xF3, 0x44, 0x0F, 0x11, 0x0D, 0xA5, 0x00, 0x00, 0x00, 
					0x50, 0x48, 0xA1, 0x43, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x89, 0x41, 0x40, 
					0x48, 0xA1, 0x47, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x89, 0x41, 0x44, 
					0x48, 0xA1, 0x4B, CaveByte[1], CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x48, 0x89, 0x41, 0x48, 0x58, 0xE9, 0x49, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x7A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x83, 0x3D, 0x5B, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x32, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0x4C, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x40, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0x41, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x44, 0xF3, 0x44, 0x0F, 0x10, 0x0D, 0x36, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x11, 0x49, 0x48, 0xE9, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x44, 0x0F, 0x10, 0x49, 0x24 };

				CodeCave[1] = (long)LibMem.CreateCodeCave(AoBScan[2].ToString("X"), asm, 6);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + Velocity + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}

		public async Task CheatFreezeAI()
        {
			AoBScan[3] = 0; CodeCave[2] = 0;

			AoBScan[3] = (await LibMem.AoBScan(minRange, maxRange, FreezeAI, false, false, true)).FirstOrDefault();

			if (AoBScan[3] > 0)
			{
				if (CodeCave[1] == 0)
					await CheatVelocity();

				if (CodeCave[1] == 0)
					return;

				byte[] CaveByte = BitConverter.GetBytes(CodeCave[1]);

				//MessageBox.Show(BitConverter.ToString(CaveByte));

				byte[] asm = { 0x50, 0x48, 0xA1, 0x9D, 0x02, CaveByte[2], CaveByte[3], CaveByte[4], CaveByte[5], CaveByte[6], CaveByte[7], 0x83, 0x3D, 0x27, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x13, 0x00, 0x00, 0x00, 0x48, 0x39, 0xC8, 0x0F, 0x84, 0x0A, 0x00, 0x00, 0x00, 0xC7, 0x81, 0xAC, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x58, 0xF3, 0x0F, 0x10, 0x89, 0xAC, 0x06, 0x00, 0x00 };

				CodeCave[2] = (long)LibMem.CreateCodeCave(AoBScan[3].ToString("X"), asm, 8);
			}
			else
				MessageBox.Show("Address not found!\nPlease try again to Activate Cheat or try to restart the game and trainer.\n\nIf this error still occur, please (Press Ctrl+C) to copy, and contact me at fearlessrevolution.com.\n\n" + "Process ID: " + pID + "\nProcess Name: " + ProcName + "\nSearch Range: " + minRange.ToString("X") + " - " + maxRange.ToString("X") + "\nSignature: " + FreezeAI + "\n\nTrainer Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Error", 0, MessageBoxIcon.Error);
		}
	}
}