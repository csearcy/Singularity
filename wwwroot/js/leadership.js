const Roles = {
	GuildLeader: 0,
	Officer: 1,
	RaidLeader: 3,
	Banker: 4,
	Raider: 6,
	SecondaryRaider: 40
}

const RAID_EXPANSION_ID = 514; // On new expansion release, run Journal Expansions Index API (https://develop.battle.net/documentation/world-of-warcraft/game-data-apis) to get new expansion ID number.
const RAID_DIFFICULTY_MODE = "Heroic";
const SECONDARY_NAME_SLUG = "hphelion";
const RAIDERS_PER_ROW = 3;
const CELLS_PER_RAIDER_BOX = 3;
const CHARACTER_TABLE_PREFIX = "character-table-";

var accessToken = "";
var seasonId = 0;

$(document).ready(function() {
    LoadPage(RETRIES);
});

function LoadPage(retries) {
    GenerateAccessToken()
    .done(function (accessTokenData) {
        accessToken = accessTokenData.access_token;
        GetMembersDataForLoad(RETRIES);
    })
    .fail(function(xhr, textStatus, errorThrown ) {
        if (retries > 0) {
            setTimeout(LoadPage(retries-1), 1000);
            return;
        }
        return;
    });
}

function GetMembersDataForLoad(retries) {
    GetMembersData(NAME_SLUG)
    .done(function(membersData) {
        GetMythicKeystoneSeasonsIndex(membersData, RETRIES);
    })
    .fail(function(xhr, textStatus, errorThrown ) {
        if (retries > 0) {
            setTimeout(GetMembersDataForLoad(retries-1), 1000);
            return;
        }
        return;
    });
}

function GenerateAccessToken() {
    return $.ajax({
        url: "https://oauth.battle.net/token",
        type: "POST",
        data: {
            grant_type: "client_credentials"
        },
        beforeSend: function(xhr) {
            xhr.setRequestHeader("Authorization", "Basic " + btoa(CLIENT_ID + ":" + CLIENT_SECRET));
        }
    });
}

function GetMembersData(nameSlug) {
    return $.ajax({
        url: "https://us.api.blizzard.com/data/wow/guild/" + REALM_SLUG + "/"  + nameSlug +"/roster?namespace=profile-us&locale=en_US&access_token=" + accessToken,
        type: "GET"
    });
}

function GetMythicKeystoneSeasonsIndex(membersData, retries) {
    GetMythicKeystoneSeasonsIndexData()
    .done(function (mythicSeasonData) {
        seasonId = mythicSeasonData.current_season.id;
        LoadGrids(membersData);
    })
    .fail(function(xhr, textStatus, errorThrown ) {
        if (retries > 0) {
            setTimeout(GetMythicKeystoneSeasonsIndex(retries-1), 1000);
            return;
        }
        return;
    });
}

function GetMythicKeystoneSeasonsIndexData() {
    return $.ajax({
        url: "https://us.api.blizzard.com/data/wow/mythic-keystone/season/index?namespace=dynamic-us&locale=en_US&access_token=" + accessToken,
        type: "GET"
    })
}

function LoadGrids(rosterData) {
    let guildMembers = rosterData.members.filter(function(member) {
        return member.rank === Roles.GuildLeader || member.rank === Roles.Officer || member.rank === Roles.RaidLeader || member.rank === Roles.Banker;
    });

    guildMembers.forEach(m => m.faction = rosterData.guild.faction.type);

    let guildLeader = guildMembers.filter(function(member) {
        return member.rank === Roles.GuildLeader;
    });
    
    
    SetRoleDiv(guildLeader, "leader");

    let officers = guildMembers.filter(function(member) {
        return member.rank === Roles.Officer;
    });
    
    SetRoleDiv(officers, "officer");
    
    let raidLeader = guildMembers.filter(function(member) {
        return member.rank === Roles.RaidLeader;
    });
    
    
    SetRoleDiv(raidLeader, "rleader");

    let bankers = guildMembers.filter(function(member) {
        return member.rank === Roles.Banker;
    });
    
    SetRoleDiv(bankers, "banker");
}

function SetRoleDiv(members, role) {
    let wrapperDiv = document.getElementById("race-wrapper");
    let titleDiv = document.createElement("div");
    titleDiv.classList.add("title-bar");

    let header = document.createElement("h1");
    header.classList.add("title-text2");
    var title = GetTitle(role);
    header.innerHTML = title;
    titleDiv.appendChild(header);
    wrapperDiv.appendChild(titleDiv);
    
    
    for(let i=0; i<members.length; i++) {
        let memberDiv = document.createElement("div");
        memberDiv.classList.add("team-div");        
        wrapperDiv.appendChild(memberDiv);
        
        let member = members[i];
        let memberName = member.character.name;
        
        let portraitDiv = document.createElement("div");
        portraitDiv.classList.add("portrait");
        
        let avatarImage = document.createElement("img");
        avatarImage.classList.add("player-portrait");
        let count = i+1;
        avatarImage.id = "avatar-image-" + role + count;
        portraitDiv.appendChild(avatarImage);
        
        memberDiv.appendChild(portraitDiv);
        
        let overlayDiv = document.createElement("div");
        overlayDiv.classList.add("player-overlay");
        memberDiv.appendChild(overlayDiv);
      
        GetMediaData(member, avatarImage.id, RETRIES);
        
        let infoDiv = document.createElement("div");
        infoDiv.classList.add("player-info");
        memberDiv.appendChild(infoDiv);
        SetDataTable(member, infoDiv);
    }
}

function GetTitle(role) {
    if(role === "leader") {
        return "Leader";
    }
    
    if(role === "officer") {
        return "Officers";
    }
    
    if(role === "rleader") {
        return "Raid Leaders";
    }
    
    if(role === "banker") {
        return "Treasurer";
    }
    
    if(role === "raider") {
        return "Raid Team";
    }
    
    return null;
}

function GetMediaData(member, avatarImageId, retries) {
    GetCharacterMediaSummaryData(member)
        .done(function(response) {
            SetImageSource(avatarImageId, response);
        })
        .fail(function(xhr, textStatus, errorThrown ) {
            if (retries > 0) {
                setTimeout(GetMediaData(member, avatarImageId, retries-1), 1000);
                return;
            }
            return;
        });
}

function GetCharacterMediaSummaryData(member) {
    let memberName = member.character.name.toLowerCase();
    return $.ajax({
        url: "https://us.api.blizzard.com/profile/wow/character/" + REALM_SLUG + "/" + memberName + "/character-media?namespace=profile-us&locale=en_US&access_token=" + accessToken,
        type: "GET"
    });
}

function SetImageSource(id, response) {
    let imageUrl = response?.assets?.find(e => e.key === "avatar")?.value;
            
    if(imageUrl === undefined) {
        imageUrl = response?.avatar_url;
        if(imageUrl === undefined) {
            return;
        }
    }
            
    document.getElementById(id).src = imageUrl;
}

function SetDataTable(member, parent) {
    let memberName = member.character.name;
    CreateCharacterDataTable(member, parent);
    AddCharacterData(member, RETRIES);
}

function CreateCharacterDataTable(member, parent) {
    const characterTable = document.createElement('table');
    characterTable.classList.add("rosterbg");
    var memberName = member.character.name;
    characterTable.id = CHARACTER_TABLE_PREFIX + memberName;
    
    let nameDiv = document.createElement("div");
    nameDiv.id = "member-name-div-" + memberName;
    nameDiv.innerHTML = memberName;
    nameDiv.classList.add("rostername");
    parent.appendChild(nameDiv);
    
    for (let i = 0; i < 2; i++) {
        const row = characterTable.insertRow();
        for (let j = 0; j < 6; j++) {
            const cell = row.insertCell();
            
            if (j%2 === 0) {
                cell.classList.add("rosterinfotitle");
                continue;
            } 
            
            cell.classList.add("rosterinfo");
        }
    }
  
    parent.appendChild(characterTable);
}

function AddCharacterData(member, retries) {
    let memberName = member.character.name;
    
    GetCharacterProfileSummaryData(member).done(function(characterProfileSummaryData) {
        GetEncounterData(member, characterProfileSummaryData, RETRIES);
    })
    .fail(function (xhr) {
        if (retries > 0) {
            setTimeout(AddCharacterData(member, retries-1), 1000);
            return;
        }
        return;
    });
}

function GetEncounterData(member, characterProfileSummaryData, retries) {
    let memberName = member.character.name;
    
    GetCharacterEncounterData(member).done(function(characterEncounterData) {
        let expansions = characterEncounterData.expansions.filter(function(expansion) {
            return expansion.expansion.id === RAID_EXPANSION_ID;
        });
        
        let instance = expansions[0]?.instances.filter(function (instance) { 
            return instance.instance.name === RAID_NAME; 
        })[0]?.modes.filter(function (mode) {
                return mode.difficulty.name === RAID_DIFFICULTY_MODE;
        });
        
        let raidProgress = "N/A";
        if(instance !== undefined  && instance.length > 0) {
            let completed = instance[0].progress.completed_count;
            let total = instance[0].progress.total_count;
            raidProgress = completed + "/" + total + " H";
        } 
        
        SetCharacterProfileCells(characterProfileSummaryData, memberName, raidProgress, RETRIES);
    })
    .fail(function (xhr) {
        if (retries > 0) {
            setTimeout(GetEncounterData(member, characterProfileSummaryData, retries-1), 1000);
            return;
        }
        return;
    });
}

function GetCharacterProfileSummaryData(member) {
    return $.ajax({
        url: "https://us.api.blizzard.com/profile/wow/character/" + REALM_SLUG + "/" + member.character.name.toLowerCase() + "?namespace=profile-us&locale=en_US&access_token=" + accessToken,
        type: "GET"
    }); 
}

function GetCharacterEncounterData(member) {
    return $.ajax({
        url: "https://us.api.blizzard.com/profile/wow/character/" + REALM_SLUG + "/" + member.character.name.toLowerCase() + "/encounters/raids?namespace=profile-us&locale=en_US&access_token=" + accessToken,
        type: "GET"
    }); 
}

function SetCharacterProfileCells(profileData, memberName, raidProgress, retries) {
    let table = document.getElementById(CHARACTER_TABLE_PREFIX + memberName);
    
    GetCharacterMythicKeystoneSeasonData(memberName)
    .done(function (characterSeasonData)
    {
        table.rows[0].cells[5].innerHTML = Math.round(characterSeasonData.mythic_rating.rating);
    })
    .always(function (characterSeasonData) 
    {
        if(profileData?.race?.name === undefined) {
            return;
        }
        
        table.rows[0].cells[0].innerHTML = "Class:";
	    table.rows[1].cells[0].innerHTML = "Race:";
	    table.rows[0].cells[2].innerHTML = "Spec:";
	    table.rows[0].cells[4].innerHTML = "Mythic+:";
	    table.rows[1].cells[2].innerHTML = "Faction:";
	    table.rows[1].cells[4].innerHTML = "Raid:";
        table.rows[0].cells[1].innerHTML = profileData.character_class.name;
        table.rows[0].cells[3].innerHTML = profileData.active_spec.name;
        table.rows[1].cells[1].innerHTML = profileData.race.name;
        table.rows[1].cells[3].innerHTML = profileData.faction.name;
	    table.rows[1].cells[5].innerHTML = raidProgress;
        
        let nameDiv = document.getElementById("member-name-div-" + memberName);
        nameDiv.classList.add(profileData.character_class.name.replace(/ /g, "-"));
    })
    .fail(function(xhr) {
        if(xhr.status==404) {
            table.rows[0].cells[5].innerHTML = "N/A";
            return;
        };
        
        if (retries > 0) {
            setTimeout(SetCharacterProfileCells(profileData, memberName, raidProgress, retries-1), 1000);
            return;
        }
        return;
    });
}

function GetCharacterMythicKeystoneSeasonData(memberName) {
    return $.ajax({
        url: "https://us.api.blizzard.com/profile/wow/character/" + REALM_SLUG + "/" + memberName.toLowerCase() + "/mythic-keystone-profile/season/" + seasonId + "?namespace=profile-us&locale=en_US&access_token=" + accessToken,
        type: "GET",
    });
}