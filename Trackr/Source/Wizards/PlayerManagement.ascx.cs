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
        private List<PlayerPass> PlayerPasses
        {
            get { return ViewState["PlayerPasses"] as List<PlayerPass>; }
            set { ViewState["PlayerPasses"] = value; }
        }
        private List<TeamPlayer> TeamPlayers
        {
            get { return ViewState["TeamPlayers"] as List<TeamPlayer>; }
            set { ViewState["TeamPlayers"] = value; }
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




        #region Team Administration
        public IQueryable gvTeamAssignments_GetData()
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<PlayerPass>(i=>i.TeamPlayers);
                fetch.LoadWith<TeamPlayer>(i=>i.Team);
                fetch.LoadWith<Team>(i=>i.Program);

                TeamPlayers = ppc.GetWhere(i => i.PlayerID == PrimaryKey.Value && i.TeamPlayers.Count() > 0, fetch).SelectMany(i => i.TeamPlayers).ToList();

                return TeamPlayers.Select(i => new
                {
                    TeamName = i.Team.TeamName,
                    ProgramName = i.Team.Program.ProgramName,
                    Season = string.Format("{0:yyyy} - {1:yy}", i.Team.StartYear.Year, i.Team.EndYear.Year),
                    IsSecondary = i.IsSecondary,
                    StartYear = i.Team.StartYear,
                    IsRemovable = DateTime.Now < i.Team.StartYear,
                }).OrderByDescending(i => i.StartYear).ThenBy(i => i.ProgramName).ThenBy(i => i.TeamName).AsQueryable();
            }
        }


        #endregion


        #region Pass Administration
        private void ClearPlayerPassForm()
        {
            PlayerPicture = null;
            txtPassExpires.Text = "";
            divPreview.Visible = false;
            pnlAddEditPass.Visible = false;
        }

        private void SetPreviewImage(byte[] data)
        {
            imgUploadPreview.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(data);
            divPreview.Visible = data != null && data.Length > 0;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            byte[] array = uploadPlayerPass.FileBytes;

            if (array != null)
            {
                PlayerPicture = array;
                SetPreviewImage(array);
            }
        }

        protected void lnkReloadImage_Click(object sender, EventArgs e)
        {
            if (PlayerPicture != null)
            {
                SetPreviewImage(PlayerPicture);
            }
        }

        public IQueryable gvPlayerPasses_GetData()
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                PlayerPasses = ppc.GetWhere(i => i.PlayerID == PrimaryKey.Value).ToList();

                return PlayerPasses.Select(i => new
                {
                    Expiration = i.Expires,
                    PassNumber = i.PassNumber,
                    PlayerPassID = i.PlayerPassID,
                    Editable = DateTime.Today < i.Expires,
                }).OrderByDescending(i => i.Expiration).AsQueryable();
            }
        }

        protected void gvPlayerPasses_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                PlayerPass pass = ppc.GetWhere(i => i.PlayerPassID == PlayerPasses[e.NewEditIndex].PlayerPassID, fetch).First();
                ClearPlayerPassForm();
                gvPlayerPasses.EditIndex = e.NewEditIndex;
                gvPlayerPasses.DataBind();

                // populate
                txtPassExpires.Text = pass.Expires.ToString("yyyy-MM-dd");
                PlayerPicture = pass.Photo;

                if (pass.Photo != null && pass.Photo.Count() > 0)
                {
                    SetPreviewImage(pass.Photo);
                }

                pnlAddEditPass.Visible = true;
            }
        }

        protected void gvPlayerPasses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPlayerPasses.EditIndex = -1;
            gvPlayerPasses.DataBind();
        }

        private void SavePlayerPass(int? playerPassID)
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                PlayerPass playerPass = playerPassID.HasValue ? ppc.Get(playerPassID.Value) : new PlayerPass();

                playerPass.Photo = PlayerPicture;
                playerPass.PlayerID = PrimaryKey.Value;

                if (!playerPassID.HasValue)
                {
                    playerPass.Expires = DateTime.Parse(txtPassExpires.Text);

                    PlayerPass inserted = ppc.AddNew(playerPass);
                    playerPassID = inserted.PlayerPassID;
                    AlertBox.SetStatus("Successfully saved new player pass.");
                }
                else
                {
                    ppc.Update(playerPass);
                    AlertBox.SetStatus("Successfully saved existing player pass.");
                }

                PlayerPicture = null;
                pnlAddEditPass.Visible = false;
            }
        }
        #endregion

    }
}