namespace DOL.GS.Commands
{
	/// <summary>
	/// Providing some basic command handler functionality
	/// </summary>
	public abstract class AbstractCommandHandler
	{
		/// <summary>
		/// Is this player spamming this command
		/// </summary>
		/// <param name="player"></param>
		/// <param name="commandName"></param>
		/// <returns></returns>
		public static bool IsSpammingCommand(GamePlayer player, string commandName)
		{
			return IsSpammingCommand(player, commandName, ServerProperties.Properties.COMMAND_SPAM_DELAY);
		}

		/// <summary>
		/// Is this player spamming this command
		/// </summary>
		/// <param name="player"></param>
		/// <param name="commandName"></param>
		/// <param name="delay">How long is the spam delay in milliseconds</param>
		/// <returns>true if less than spam protection interval</returns>
		public static bool IsSpammingCommand(GamePlayer player, string commandName, int delay)
		{
			if ((ePrivLevel) player.Client.Account.PrivLevel > ePrivLevel.Player)
				return false;

			string spamKey = commandName + "NOSPAM";
			long tick = player.TempProperties.GetProperty<long>(spamKey);

			if (tick > 0 && player.CurrentRegion.Time - tick <= 0)
			{
				player.TempProperties.RemoveProperty(spamKey);
			}

			long changeTime = player.CurrentRegion.Time - tick;

			if (tick > 0 && (player.CurrentRegion.Time - tick) < delay)
			{
				return true;
			}

			player.TempProperties.SetProperty(spamKey, player.CurrentRegion.Time);
			return false;
		}

		public virtual void DisplayMessage(GamePlayer player, string message)
		{
			DisplayMessage(player.Client, message, new object[] {});
		}

		public virtual void DisplayMessage(GameClient client, string message)
		{
			DisplayMessage(client, message, new object[] {});
		}

		public virtual void DisplayMessage(GameClient client, string message, params object[] objs)
		{
			if (client == null || !client.IsPlaying)
				return;

			ChatUtil.SendSystemMessage(client, string.Format(message, objs));
			return;
		}

		public virtual void DisplaySyntax(GameClient client)
		{
			if (client == null || !client.IsPlaying)
				return;

			var attrib = (CmdAttribute[]) GetType().GetCustomAttributes(typeof (CmdAttribute), false);
			if (attrib.Length == 0)
				return;

			// If a value isn't found for a command type header/divider, ignore it
			if (string.IsNullOrEmpty(attrib[0].Header))
			{
				// Include command description at head of list upon typing '/example'
				ChatUtil.SendCommMessage(client, attrib[0].Description, null);
			}
			else
			{
				// Include header/divider at head of return upon typing the main command identifier or alias (e.g., '/command')
				ChatUtil.SendHeaderMessage(client, attrib[0].Header, null);

				// Include main command type description below separator
				ChatUtil.SendCommMessage(client, attrib[0].Description, null);
			}
			
			// Run for each value found under "params usage" until the whole command list is displayed
			foreach (var sentence in attrib[0].Usage)
			{
				// To contrast the appearance of command syntax against their descriptions, include ".Syntax." in the translation ID (e.g., "AdminCommands.Account.Syntax.AccountName")
				if (sentence.Contains(".Syntax."))
				{
					ChatUtil.SendSyntaxMessage(client, sentence, null);
				}
				// All other values display as command descriptions (i.e., CT_System)
				else
				{
					ChatUtil.SendCommMessage(client, sentence, null);	
				}
			}
		}

		public virtual void DisplaySyntax(GameClient client, string subcommand)
		{
			if (client == null || !client.IsPlaying)
				return;

			var attrib = (CmdAttribute[]) GetType().GetCustomAttributes(typeof (CmdAttribute), false);

			if (attrib.Length == 0)
				return;

			foreach (string sentence in attrib[0].Usage)
			{
				string[] words = sentence.Split(new[] {' '}, 3);

				if (words.Length >= 2 && words[1].Equals(subcommand))
				{
					ChatUtil.SendSystemMessage(client, sentence, null);
				}
			}

			return;
		}

		public virtual void DisplaySyntax(GameClient client, string subcommand1, string subcommand2)
		{
			if (client == null || !client.IsPlaying)
				return;

			var attrib = (CmdAttribute[]) GetType().GetCustomAttributes(typeof (CmdAttribute), false);

			if (attrib.Length == 0)
				return;

			foreach (string sentence in attrib[0].Usage)
			{
				string[] words = sentence.Split(new[] {' '}, 4);

				if (words.Length >= 3 && words[1].Equals(subcommand1) && words[2].Equals(subcommand2))
				{
					ChatUtil.SendSystemMessage(client, sentence, null);
				}
			}

			return;
		}
	}
}
