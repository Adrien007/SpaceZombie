//LevelTransitionManager.cs

using Godot;
using SpaceZombie.Events;
using SpaceZombie.Ui;

namespace SpaceZombie.Niveaux
{
    public class LevelTransitionManager
    {
        private ProchainNiveauUi prochainNiveauUi;
        private LevelManager lm;

        public LevelTransitionManager(ProchainNiveauUi prochainNiveauUi, LevelManager lm)
        {
            this.prochainNiveauUi = prochainNiveauUi;
            this.lm = lm;
            GameEvents.Instance.EndLevel += ChangerNiveauLogic;
            prochainNiveauUi.timer.Timeout += WaitForTimerToFinish;
        }

        public void ChangerNiveauLogic()
        {
            if (!GodotObject.IsInstanceValid(prochainNiveauUi)) return;
            
            prochainNiveauUi.ProcessMode = Node.ProcessModeEnum.Always;
            prochainNiveauUi.UpdateLabelTexte(lm.level.ToString());
            prochainNiveauUi.Visible = true;
            prochainNiveauUi.StartTimer();
        }


        private void WaitForTimerToFinish()
        {
            if (!GodotObject.IsInstanceValid(prochainNiveauUi)) return;
            
            lm.CreerNiveau();
            prochainNiveauUi.ProcessMode = Node.ProcessModeEnum.Disabled;
            prochainNiveauUi.Visible = false;
        }
    }
}
