﻿using Aurora.Devices;
using Aurora.EffectsEngine;
using Aurora.Profiles.Minecraft.GSI;
using Aurora.Settings.Layers;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace Aurora.Profiles.Minecraft.Layers {

    public class MinecraftKeyConflictLayerProperties : LayerHandlerProperties2Color<MinecraftKeyConflictLayerProperties> {

        // PrimaryColor -> No conflict
        // SecondaryColor -> Hard conflict
        // TertiaryColor -> Soft conflict

        [Newtonsoft.Json.JsonIgnore]
        public Color TertiaryColor => _TertiaryColor ?? Color.Empty;
        public Color? _TertiaryColor { get; set; }

        public MinecraftKeyConflictLayerProperties() : base() { }
        public MinecraftKeyConflictLayerProperties(bool assign_default = false) : base(assign_default) { }

        public override void Default() {
            base.Default();
            _PrimaryColor = Color.FromArgb(0, 255, 0);
            _SecondaryColor = Color.FromArgb(255, 0, 0);
            _TertiaryColor = Color.FromArgb(255, 127, 0);
        }
    }


    public class MinecraftKeyConflictLayerHandler : LayerHandler<MinecraftKeyConflictLayerProperties> {
        private readonly EffectLayer _layer = new("Minecraft Key Conflict Layer");
        private readonly SolidBrush _backgroundBrush = new(Color.Black);
        private bool _clear = true;

        protected override UserControl CreateControl() {
            return new Control_MinecraftKeyConflictLayer(this);
        }

        public override EffectLayer Render(IGameState gameState) {
            if (gameState is not GameState_Minecraft minecraftState || !minecraftState.Game.ControlsGuiOpen)
            {
                if (!_clear)
                {
                    _layer.Clear();
                    _clear = true;
                }
                return _layer;
            }
            _clear = false;

            _layer.Fill(_backgroundBrush); // Hide any other layers behind this one
            // Set all keys in use by any binding to be the no-conflict colour
            foreach (var kb in minecraftState.Game.KeyBindings)
                if(kb!=null)
                    _layer.Set(kb.AffectedKeys, Properties.PrimaryColor);

            // Override the keys for all conflicting keys
            foreach (var kvp in CalculateConflicts(minecraftState))
                _layer.Set(kvp.Key, kvp.Value ? Properties.TertiaryColor : Properties.SecondaryColor);
            return _layer;
        }

        /// <summary>
        /// Method that calculates the key conflicts based on the GameState's Game.KeyBindings property.
        /// Returns an enumerable of all DeviceKeys with a conflict, and whether they are only modifier conflicts (warning).
        /// Forge shows modifier conflicts in red and soft in orange on the in-game keys menu.
        /// </summary>
        private Dictionary<DeviceKeys, bool> CalculateConflicts(GameState_Minecraft state) {
            Dictionary<DeviceKeys, bool> keys = new Dictionary<DeviceKeys, bool>();
            foreach (var bind in state.Game.KeyBindings) { // For every key binding

                // This code is based on the code from Minecraft in "GuiKeyBindingList.java" in the "drawEntry" method.
                // It may not be the most efficient way of computing conflicts but I'm struggling to entirely follow
                // the logic in the Minecraft code, so I've decided to replicate it to prevent conflicts
                bool hasConflict = false;
                bool isOnlyModifierConflict = true;

                foreach (var otherBind in state.Game.KeyBindings) { // Check against every other key binding
                    if (bind != null && otherBind != null)
                    {
                        if (bind != otherBind && otherBind.ConflictsWith(bind))
                        {
                            hasConflict = true;
                            isOnlyModifierConflict &= otherBind.ModifierConflictsWith(bind);
                        }
                    }
                }
                // End replicated section

                if (hasConflict)
                    foreach (var affectedKey in bind.AffectedKeys) // For each key that is affected by this keybind
                        keys[affectedKey] = keys.ContainsKey(affectedKey) // Check if this key is already flagged as a conflict
                            ? keys[affectedKey] && isOnlyModifierConflict // If so, ensure it shows full conflicts over modifier conflicts 
                            : isOnlyModifierConflict; // Else if not already flagged, simply set it.
            }
            return keys;
        }
    }
}
