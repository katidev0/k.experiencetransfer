using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;

namespace KExperienceTransfer
{
    public class KExperienceTransfer : RocketPlugin<KExperienceTransferConfiguration>
    {
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "command_usage", "Usage: /transfer <player> <amount>" },
            { "player_not_found", "Player '{0}' not found!" },
            { "invalid_amount", "Invalid amount entered!" },
            { "not_enough_experience", "You don't have enough experience to transfer!" },
            { "transfer_success_sender", "You sent {0} XP to {1}!" },
            { "transfer_success_target", "You received {0} XP from {1}!" }
        };

        protected override void Load()
        {
            Logger.Log($"{Name} has been loaded!");
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!");
        }

        [RocketCommand("transfer", "Transfer experience to another player", "<player> <amount>", AllowedCaller.Player)]
        [RocketCommandPermission("ktransfer.use")]
        public void ExecuteCommand(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer sender = (UnturnedPlayer)caller;

            if (args.Length < 2)
            {
                UnturnedChat.Say(sender, Translate("command_usage"));
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(args[0]);
            if (target == null)
            {
                UnturnedChat.Say(sender, Translate("player_not_found", args[0]));
                return;
            }

            if (!uint.TryParse(args[1], out uint amount))
            {
                UnturnedChat.Say(sender, Translate("invalid_amount"));
                return;
            }

            if (sender.Experience < amount)
            {
                UnturnedChat.Say(sender, Translate("not_enough_experience"));
                return;
            }

            sender.Experience -= amount;
            target.Experience += amount;

            UnturnedChat.Say(sender, Translate("transfer_success_sender", amount, target.DisplayName));
            UnturnedChat.Say(target, Translate("transfer_success_target", amount, sender.DisplayName));
        }
    }

    public class KExperienceTransferConfiguration : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
        }
    }
}
