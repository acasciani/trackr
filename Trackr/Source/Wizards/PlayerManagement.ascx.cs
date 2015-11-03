using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr.Source.Wizards
{
    public partial class PlayerManagement : WizardBase<int>
    {
        private byte[] PlayerPicture
        {
            get { return Session["PlayerPicture"] as byte[]; }
            set { Session["PlayerPicture"] = value; }
        }

        private int? PlayerPassID
        {
            get { return ViewState["PlayerPassID"] as int?; }
            set { ViewState["PlayerPassID"] = value; }
        }

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
            PlayerPicture = null;
        }

        private void Populate_Edit()
        {
            using (PlayersController pc = new PlayersController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Player>(i => i.PlayerPasses);
                fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);
                fetch.LoadWith<TeamPlayer>(i => i.Team);
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                Player player = pc.GetScopedEntity(CurrentUser.UserID, (WasNew ? Permissions.PlayerManagement.CreatePlayer : Permissions.PlayerManagement.EditPlayer), PrimaryKey.Value, fetch);
                txtFirstName.Text = player.FName;
                txtLastName.Text = player.LName;
                txtMiddleInitial.Text = player.MInitial.HasValue ? player.MInitial.Value.ToString() : "";

                txtDateOfBirth.Text = player.DateOfBirth.ToString("yyyy-MM-dd");

                // Load player pass info, note there should only be one there should be a constraint on the player id and expiration date
                PlayerPass playerPass = player.PlayerPasses.Where(i => DateTime.Today <= i.Expires).FirstOrDefault();

                divPreview.Visible = false;
                PlayerPassID = null;
                if (playerPass != null)
                {
                    PlayerPassID = playerPass.PlayerPassID;

                    if (playerPass.Photo != null)
                    {
                        PlayerPicture = playerPass.Photo;
                        SetPreviewImage(playerPass.Photo);
                    }
                    else
                    {
                        PlayerPicture = null;
                    }
                }

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

        private void Save_Step2()
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                PlayerPass playerPass = PlayerPassID.HasValue ? ppc.Get(PlayerPassID.Value) : new PlayerPass();

                playerPass.Photo = PlayerPicture;
                playerPass.PlayerID = PrimaryKey.Value;

                if (!PlayerPassID.HasValue)
                {
                    playerPass.Expires = new DateTime(2016, 8, 1);

                    PlayerPass inserted = ppc.AddNew(playerPass);
                    PlayerPassID = inserted.PlayerPassID;
                    AlertBox.SetStatus("Successfully saved new player pass.");
                }
                else
                {
                    ppc.Update(playerPass);
                    AlertBox.SetStatus("Successfully saved existing player pass.");
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

                case 1:
                    Save_Step2();
                    break;

                default: break;
            }
        }

        protected void validatorDOBParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime dob;
            args.IsValid = DateTime.TryParse(args.Value, out dob);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            byte[] array = uploadPlayerPass.FileBytes;
            PlayerPicture = array;
            SetPreviewImage(array);
        }

        private void SetPreviewImage(byte[] data)
        {
            imgUploadPreview.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(data);
            divPreview.Visible = data != null;
        }
    }
}