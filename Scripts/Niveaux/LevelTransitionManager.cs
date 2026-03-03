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
        private ResetEtatManager res;
        private SceneTree sceneTree;

        private bool finCreationNiveau = false;

        public LevelTransitionManager(SceneTree sceneTree, ProchainNiveauUi prochainNiveauUi, LevelManager lm,
                                        ResetEtatManager res)
        {
            this.sceneTree = sceneTree;
            this.prochainNiveauUi = prochainNiveauUi;
            this.lm = lm;
            this.res = res;

            //endLevelSystemEnemySide.EndLevelSignal += ChangerNiveauLogic;
            GameEvents.Instance.EndLevel += ChangerNiveauLogic;
            lm.FinCreationNiveau += FinCreationNiveau;
            prochainNiveauUi.timer.Timeout += WaitForTimerOrLevelToFinish;
        }
        private void FinCreationNiveau()
        {
            finCreationNiveau = true;
            WaitForTimerOrLevelToFinish();
        }
        public void ChangerNiveauLogic()
        {
            //Ecran loading
            prochainNiveauUi.UpdateLabelTexte(lm.GlobalLevel.ToString());
            prochainNiveauUi.Visible = true;
            prochainNiveauUi.StartTimer();

            //Desactiver input
            sceneTree.Paused = true;

            //Resetter les etats
            res.ResetToInitaialState();

            //Settuper prochain niveau
            lm.CreerNiveau();

            //Suite dans WaitForTimerOrLevelToFinish
            //Ecran loading Fin
            //Activer Timer
            //ReactiverInput
        }
        private void WaitForTimerOrLevelToFinish()
        {
            //GD.Print($"{finCreationNiveau} || {prochainNiveauUi.TimeLeft()}");
            if (!finCreationNiveau || prochainNiveauUi.TimeLeft() > 0)
                return;

            finCreationNiveau = false;
            //Ecran loading Fin
            prochainNiveauUi.Visible = false;
            //Activer Timer
            res.StartTimerState();
            //ReactiverInput
            sceneTree.Paused = false;
        }
    }
}