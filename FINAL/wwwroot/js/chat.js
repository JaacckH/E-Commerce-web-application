
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.start();
var recip;
var prev;
var community;
var flair;
var cycle = 2;
var chatrecent;

connection.on("ReceiveMessage", function (user, recipient, message) {
    if ((getSessionID() == recipient && user != recip) || recip == null) {
        return;
    }
    document.getElementById("messagesList").innerHTML += message;
    document.getElementById("messagesList").scrollTop = document.getElementById("messagesList").scrollHeight;
});

connection.on("ContentDelivery", function (content, div) {
    document.getElementById(div).innerHTML = content;
    document.getElementById(div).scrollTop = document.getElementById(div).scrollHeight;
});

connection.on("InputDelivery", function (content, id, type) {
    console.log("the type of value input is: " + type + " the content is: " + content + " the ID is: " + id);
    if (type == true) {
        document.getElementById(id).value = content;

        FavSelectionLoad(false, null)
        
    } else {
        document.getElementById(id).selectedIndex = content;
    }
});


connection.on("UpdateContent", function (content, div) {
    document.getElementById(div).innerHTML += content;
    document.getElementById(div).scrollTop = document.getElementById(div).scrollHeight;
});

connection.on("copyInvite", function (val) {
    var dummy = document.createElement("textarea");
    document.body.appendChild(dummy);
    dummy.value = "https://" + window.location.host + "/Invite?id=" + val;
    dummy.select();
    document.execCommand("copy");
    document.body.removeChild(dummy);
});

connection.on("ProgressCreateAccount", function (Recovery) {
    toggle_create();
    $('#recovery-modal').modal('show');
    $('#Recovery-code').html(Recovery);
});

connection.on("copyPost", function (val) {
    var dummy = document.createElement("textarea");
    document.body.appendChild(dummy);
    dummy.value = "https://" + window.location.host + "/Post?id=" + val;
    dummy.select();
    document.execCommand("copy");
    document.body.removeChild(dummy);
});

connection.on("setSessionID", function (SessionID, redirect) {
    document.cookie = "SessionID=" + SessionID;

    if (redirect == true) {
        window.location.href = '/'
    }
});

connection.on("AppendFront", function (content, div) {
    var newstring = content + document.getElementById(div).innerHTML;
    document.getElementById(div).innerHTML = newstring;
});

connection.on("ShowError", function (message) {
    document.getElementById('alert-box').style.visibility = 'visible';
    document.getElementById('alert-message').innerHTML = message;
    setTimeout(hideAlert, 2500);
});

connection.on("ShowSuccess", function (message) {
    document.getElementById('success-box').style.visibility = 'visible';
    document.getElementById('success-message').innerHTML = message;
    setTimeout(hideSuccess, 2500);
});

connection.on("ShowAcknowledge", function (message) {
    document.getElementById('acknowledge-box').style.visibility = 'visible';
    document.getElementById('acknowledge-message').innerHTML = message;
    setTimeout(hideAcknowledge, 1000);
});

connection.on("RemoveContainer", function (div) {
    var elem = document.getElementById(div);
    elem.parentNode.removeChild(elem);
});

connection.on("updateProfilePicture", function (source) {
    document.getElementById('profile-picture-1').src = source;
    document.getElementById('profile-picture-2').src = source;
    console.log("ranndat");
});

connection.on("SendNotification", function (content) {
    var current = document.getElementById('notifications').innerHTML;
    current = content + current;
    document.getElementById('notifications').innerHTML = current;

    var element = document.getElementById('notif-bell');
    if (!element.classList.contains("NOTIFICATION")) {
        element.classList.add("NOTIFICATION");
    }
});

connection.on("removePostFromFeed", function (id) {
    var elem = document.getElementById("post-container-" + id);
    elem.parentNode.removeChild(elem);
});

connection.on("redirect", function (path) {
    window.location.href = path;
});

connection.on("ValueDelivery", function (content, div) {
    document.getElementById(div).value = content;
});

connection.on("CommunityDelivery", function (communityList) {
    
    FavSelectionLoad(true, communityList)

});

function requestPosts(id) {
    connection.invoke("requestPosts", getSessionID(), id, cycle);
    cycle++;
}

function UpdatingProfile() {

    var selection = document.getElementById("genderSelect").selectedIndex;

    var genderArray = [ 'PRIVATE', 'MALE', 'FEMALE', 'OTHER'];
    var gender_update = "PRIVATE";

    if (selection >= 0) {
        gender_update = genderArray[selection];
    }

    const element = document.querySelector("body");
    var colormode;

    if (element.classList.contains("darkMode")) {
        colormode = "0";
    } else if (element.classList.contains("lightMode")) {
        colormode = "1";
    }

    connection.invoke("UpdateProfile", getSessionID(), document.getElementById("settings-text-4").innerHTML, colormode, document.getElementById("settings-text-0").innerHTML, document.getElementById("fav-community").value, gender_update);
}
function DeleteAccount() {
    if (confirm("Are you sure you want delete your account?")) {
        connection.invoke("DeleteUserAccount", getSessionID());
        redirectLogout();

    } else { }
}

function UpdateUserVerification(Username) {
    connection.invoke("UpdateUserVerification", getSessionID(), Username);
}

function UpdateUserMute(Username) {
    connection.invoke("UpdateUserMute", getSessionID(), Username);
}

function UpdateUserBan(Username) {
    connection.invoke("UpdateUserBan", getSessionID(), Username);
}

function DeleteCommunity(CommunityID) {
    if (confirm("Are you sure you want delete this community?")) {
        connection.invoke("deleteCommunity", getSessionID(), CommunityID);
        document.getElementById('forCommunity-' + CommunityID).style.visibility = 'hidden';
    } else { }
}

function hideAlert() {
    document.getElementById('alert-box').style.visibility = 'hidden';
}

function hideSuccess() {
    document.getElementById('success-box').style.visibility = 'hidden';
}

function hideAcknowledge() {
    document.getElementById('acknowledge-box').style.visibility = 'hidden';
}

function deleteCommunityPost(id, community) {
    setConnectionID();
    connection.invoke("deletePost", getSessionID(), id, community);
}

document.addEventListener("DOMContentLoaded", function (event) {
    var messageInput = document.getElementById('messageInput');

    messageInput.addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            // stop default
            sendClick();
        }
    });

});

function sendClick() {
    if (recip != null && document.getElementById('messageInput').value != "") {
        connection.invoke("SendMessage", getSessionID(), recip, document.getElementById("messageInput").value);
        document.getElementById('messageInput').value = "";
    }
}

function updateShards() {
    connection.invoke("updateShardCounters", getSessionID());
    setTimeout(updateShards, 1000);
}

window.search = search;
function search() {
    var text = document.getElementById("searchbarmain").value;
    if (text != null && text != "" && text != prev) {
        connection.invoke("searchGetUsers", text, getSessionID());
    }
    prev = text;
}

function toggleFollow(user) {
    connection.invoke("followUser", getSessionID(), user);
    toggleFollowVisual();
}

function likePost(id) {
    connection.invoke("likePost", getSessionID(), id);
    toggleLike(id);
}

function subscribe(id) {
    connection.invoke("subscribe", getSessionID(), id);
    toggleSubscribe(id);
}

function getUsers() {
    var text = document.getElementById("userSearch").value;
    if (text == "" || text == null) {
        connection.invoke("getUserList", getSessionID(), false);
    } else {
        connection.invoke("getUserList", text, true);
    }
}

function setConnectionID() {
    connection.invoke("saveConnectionID", getSessionID());
}

function loadMessages(user) {
    recip = user;
    connection.invoke("getMessages", user, getSessionID());
    document.getElementById('messageInput').placeholder = "MESSAGE " + recip.toUpperCase() + "...";
}

function getSessionID() {
    var name = "SessionID=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return null;
}

function selectPost(type) {
    connection.invoke("selectPost", getSessionID(), type);
}

function addPost(type) {
    if (type == 0) {
        var title = document.getElementById('textpost-title').value;
        var text = document.getElementById('textpost-text').value;
        community = document.getElementById('community-select-1').value;
        flair = document.getElementById('flair-select-1').value;
        connection.invoke("addTextPost", getSessionID(), title, text, community, flair);
    }

    if (type == 1) {
        var title = document.getElementById('imagepost-title').value;
        var path = document.getElementById('imagepost-image').value;
        community = document.getElementById('community-select-2').value;
        flair = document.getElementById('flair-select-2').value;
        connection.invoke("addImagePost", getSessionID(), title, path, community, flair);
    }

    if (type == 2) {
        var title = document.getElementById('videopost-title').value;
        var path = document.getElementById('videopost-url').value;
        community = document.getElementById('community-select-3').value;
        flair = document.getElementById('flair-select-3').value;
        connection.invoke("addVideoPost", getSessionID(), title, path, community, flair);
    }

    community = null;
    flair = null;
}

function addComment(id) {
    if (getSessionID() != null && getSessionID() != "") {
        var input = document.getElementById('comment-input-' + id).value;
        connection.invoke("addComment", getSessionID(), input, id);
    } else {
        redirectLogin();
    }
    document.getElementById('comment-input-' + id).value = "";
}

function ReloadPage() {
    window.location.href = "/Index";
}

function communityPromoteUser(CommunityID, user) {
    connection.invoke("communityPromoteUser", getSessionID(), CommunityID, user);
}

function createCommunity() {
    var name = document.getElementById('create-community-name').value;
    var bio = document.getElementById('create-community-bio').value;
    var type = document.getElementById('create-community-type').value;
    connection.invoke("createCommunity", getSessionID(), name, bio, type);
}

function uploadtest(id) {
    var input = document.getElementById("communitypp");
    var fReader = new FileReader();
    fReader.readAsDataURL(input.files[0]);
    fReader.onloadend = function (event) {
        var img = document.getElementById("cpp");
        document.getElementById('compp').src = event.target.result;
        img.value = event.target.result;
    }
}

function uploadImage() {
    var input = document.getElementById("imagepicker");
    var fReader = new FileReader();
    fReader.readAsDataURL(input.files[0]);
    fReader.onloadend = function (event) {
        var img = document.getElementById("imagepost-data");
        var imgElement = document.getElementById('imagepost-image');
        imgElement.src = event.target.result;
        imgElement.classList.remove('no-display');
        img.value = event.target.result;
    }
}

function communityBanUser(id, user) {
    connection.invoke("communityBanUser", getSessionID(), id, user);
}

function communityMuteUser(id, user) {
    connection.invoke("communityMuteUser", getSessionID(), id, user);
}

function LoadProfile() {
    connection.invoke("ProfileInfo", getSessionID());
}

function processShopItem(id) {
    connection.invoke("ProcessPurchase", getSessionID(), id);
}

function requestJoin(id) {
    connection.invoke("requestJoin", getSessionID(), id);
}

function acceptUser(user, id) {
    connection.invoke("acceptUser", getSessionID(), user, id);
    var elem = document.getElementById('user-request-' + user);
    elem.parentNode.removeChild(elem);
}

function leaveCommunity(id) {
    connection.invoke("leaveCommunity", getSessionID(), id);
    window.location.href = '/Community?id=' + id;
}

function clearNotifications() {
    connection.invoke("clearNotifications", getSessionID());
    var element = document.getElementById('notif-bell');
    if (element.classList.contains("NOTIFICATION")) {
        element.classList.remove("NOTIFICATION");
    }
}

function inviteUsers(id) {
    connection.invoke("copyInvite", getSessionID(), id);
}

function updateCommunity(id) {
    var visible = document.getElementById('community-settings-visibility').value.toString();
    var bio = document.getElementById('community-settings-bio').value;
    connection.invoke("updateCommunity", getSessionID(), id, visible, bio);
}

function deleteCommunity(id) {
    connection.invoke("deleteCommunity", getSessionID(), id);
    window.location.href = '/';
}

function reportCommunity(id) {
    connection.invoke("reportCommunity", getSessionID(), id);
}

function reportPost(id) {
    connection.invoke("reportPost", getSessionID(), id);
}

function reportUser(id) {
    connection.invoke("reportUser", getSessionID(), id);
}

function deleteReport(id) {
    connection.invoke("deleteReport", getSessionID(), id);
}

function deleteAppeal(id) {
    connection.invoke("deleteAppeal", getSessionID(), id);
}

function unbanUser(user, id) {
    connection.invoke("unbanUser", getSessionID(), user, id);
}

window.loginUser = loginUser;
function loginUser() {
    var username = document.getElementById('Username').value;
    var password = document.getElementById('Password').value;
    connection.invoke("loginUser", username, password, true);
}

function ignoreUser() {
    connection.invoke("ignoreUser");
}

function sharePost(id) {
    connection.invoke("sharePost", id);
}

function toggleTheme() {
    connection.invoke("toggleTheme", getSessionID()); v
}

function resetuser() {
    var Recovery = document.getElementById('RecoveryCode').value;
    var NewPass = document.getElementById('NewPassword').value;
    var ConPass = document.getElementById('ConfNewPassword').value;
    connection.invoke("ResetUser", Recovery, NewPass, ConPass);
}

function createAccount() {
    var username = document.getElementById('Username').value;
    var Displayname = document.getElementById('Display-name').value;
    var Password = document.getElementById('Password').value;
    var Email = document.getElementById('Email').value;
    var ConfirmPassword = document.getElementById('ConfirmPassword').value;
    var Agecheck = document.getElementById('imold').checked;
    connection.invoke("Createuseraccount", username, Displayname, Password, Email, ConfirmPassword, Agecheck);
}



window.FavSelectionLoad = FavSelectionLoad;
function FavSelectionLoad(Continue, communityList) {
    if ($('#fav-community').length > 0) {
        if (!Continue) {
            connection.invoke("CreateCommunityList", getSessionID());
        }else {
            var community = communityList.split(",");

            var community = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace,
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                // array of communities
                local: community
            });

            $('#fav-community').tagsinput({
                typeaheadjs: [{
                    hint: true,
                    highlight: true,
                    minLength: 1
                },
                {
                    name: 'community',
                    source: community
                }],
                freeInput: false,
                tagClass: 'badge badge-light'
            });
        }
    }
}
