const APHELION_GUILD_ID = 11653;
const RAIDS = [
    {
    "raiderIoApiName": "nerubar-palace",
    "blizzardApiName": "Nerub-ar Palace",
    "imageAbbreviation": "nerubar"
  },
    {
    "raiderIoApiName": "amirdrassil-the-dreams-hope",
    "blizzardApiName": "Amirdrassil, the Dream's Hope",
    "imageAbbreviation": "amirdrassil"
  },
  {
    "raiderIoApiName": "aberrus-the-shadowed-crucible",
    "blizzardApiName": "Aberrus, the Shadowed Crucible",
    "imageAbbreviation": "aberrus"
  },
  {
    "raiderIoApiName": "vault-of-the-incarnates",
    "blizzardApiName": "Vault of the Incarnates",
    "imageAbbreviation": "vault"
  }
];

var RAID = RAIDS[0].raiderIoApiName;
var RAID_NAME = RAIDS[0].blizzardApiName;
var RAID_ABBREVIATION = RAIDS[0].imageAbbreviation;
const DIFFICULTY = "heroic";
const CLIENT_ID = "b7f05a53efe4472fa4b3cb3d13906869";
const CLIENT_SECRET = "hcGJNA2t64KUqLt02DjS8duZ1SVusrJf";
const REALM_SLUG = "aerie-peak";
const NAME_SLUG = "aphelion";
const RETRIES = 3;

$(document).ready(function() {
    $('.nav-link').on('click', function(e) {
        e.preventDefault();
        var url = $(this).attr('href');

        $('#main-content').load(url + ' #main-content');
    });

    GetAphelionCompletionsForHeader(RETRIES);
    
    $("#logoutButton").click(function(e) {
        $.ajax({
            url:"logout",
            method:"POST",
            dataType:'json',
            data:{canLogout:true},
            success: function (data)
            {
                if(data.canLogout) {
                    alert("You have been logged out");
                    document.location.href='/login';
                } else {
                    alert("Unable to logout.");
                }
                
            },
            error: function(data) {
                alert("Unable to logout.");
            }
        });
    });
});


function sideMenu() {
  var x = document.getElementById("sideLinks");
  if (x.style.display === "block") {
    x.style.display = "none";
  } else {
    x.style.display = "block";
  }
}

function GetAphelionCompletionsForHeader(retries) {
    GetGuildRaidRankingsFromApiForHeader("heroic")
    .done(function(heroicData) {
        
    })
    .fail(function(xhr, textStatus, errorThrown ) {
        if (retries > 0) {
            setTimeout(GetAphelionCompletionsForHeader(retries-1), 1000);
            return;
        }
        return;
    });
}

function GetGuildRaidRankingsFromApiForHeader(difficulty) {
    return $.ajax({
        url: "https://raider.io/api/v1/raiding/raid-rankings?raid=" + RAID + "&difficulty=" + difficulty + "&region=us&realm=" + REALM_SLUG + "&guilds=" + APHELION_GUILD_ID + "&page=0",
        type: "GET",
        retryLimit : 3
    }); 
}