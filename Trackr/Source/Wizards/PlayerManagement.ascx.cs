using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Wizards
{
    public partial class PlayerManagement : WizardBase<int>
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            using (PlayersController pc = new PlayersController())
            using (WebUsersController wuc = new WebUsersController())
            {
                if (IsNew)
                {
                    if (!wuc.IsAllowed(CurrentUser.UserID, Permissions.PlayerManagement.CreatePlayer))
                    {
                        throw new UserUnauthorizedException("You do not have permission to create a new player.");
                    }
                }
                else
                {
                    if (pc.GetScopedEntity(CurrentUser.UserID, Permissions.PlayerManagement.EditPlayer, PrimaryKey.Value) == null)
                    {
                        throw new UserNotScopedException("You are not allowed to edit the selected player.");
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();

            if (IsPostBack)
            {
                return;
            }

            if (IsNew)
            {
                Populate_Create();
            }
            else
            {
                Populate_Edit();
            }

            // update dropdownlists
            //ddlRole.Populate(DropDownType.Role);
        }

        private void Populate_Create()
        {

        }

        private void Populate_Edit()
        {
            using (PlayersController pc = new PlayersController())
            {
                Player player = pc.GetScopedEntity(CurrentUser.UserID, (WasNew ? Permissions.PlayerManagement.CreatePlayer : Permissions.PlayerManagement.EditPlayer), PrimaryKey.Value);
                txtFirstName.Text = player.FName;
                txtLastName.Text = player.LName;
                txtMiddleInitial.Text = player.MInitial.HasValue ? player.MInitial.Value.ToString() : "";

                txtDateOfBirth.Text = player.DateOfBirth.ToString("yyyy-MM-dd");
            }
        }

        protected void lnkEditAgain_Click(object sender, EventArgs e)
        {
            PlayerWizard.ActiveStepIndex = 0;
        }

        private void Save_Step1()
        {
            using (PlayersController pc = new PlayersController())
            {
                Player player = IsNew ? new Player() : pc.Get(PrimaryKey.Value);

                player.DateOfBirth = DateTime.Parse(txtDateOfBirth.Text);
                player.FName = txtFirstName.Text;
                player.MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : txtMiddleInitial.Text.ToCharArray()[0];
                player.LName = txtLastName.Text;

                if (IsNew)
                {
                    Player inserted = pc.AddNew(player);
                    PrimaryKey = inserted.PlayerID;
                    Populate_Edit();
                    AlertBox.SetStatus("Successfully created new player.");
                }
                else
                {
                    pc.Update(player);
                    AlertBox.SetStatus("Successfully saved player information.");
                }
            }
        }

        protected void PlayerWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            switch (e.CurrentStepIndex)
            {
                case 0:
                    Save_Step1();
                    break;
                default: break;
            }
        }

        protected void validatorDOBParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime dob;
            args.IsValid = DateTime.TryParse(args.Value, out dob);
        }
    }
}