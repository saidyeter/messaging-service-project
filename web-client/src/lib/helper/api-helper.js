const apiUrl = "http://localhost:4000"//"http://localhost:4000"

const keyName = "accesstoken"
function saveAccessToken(token) {
    sessionStorage.setItem(keyName,token)
}
function getAccessToken() {
    //console.log(sessionStorage.getItem(keyName));
    return sessionStorage.getItem(keyName)
}


export async function apiLogin(username, password) {
    const path = "/auth/login"
    const data = {
        username,
        password
    }
    const res = await apiPost(path, data)
    if (res.accessToken) {
        saveAccessToken(res.accessToken)
        return true
    }
    return false
}

export async function apiRegister(displayname, email, username, password) {

    const path = "/auth/register"
    const data = {
        username,
        password,
        displayname,
        email
    }
    const res = await apiPost(path, data)
    if (res.id) {
        return true
    }
    return false
}

export function apiGetOpponents() {
    const path = `/Messages/GetOpponents`
    return apiGetWithAuth(path)
}

export function apiGetLastMessageId(opponent) {
    const path = `/Messages/GetLatestMessageBetween/${opponent}`
    return apiGetWithAuth(path)
}

export function apiGetUserInfo(opponent) {
    const path = `/Messages/GetUserInfo/${opponent}`
    return apiGetWithAuth(path)
}

export function apiGetOlderMessagesFrom(messageId) {
    const path = `/Messages/GetOlderMessagesFrom/${messageId}`
    return apiGetWithAuth(path)
}

export function apiGetSingleMessage(messageId) {
    const path = `/Messages/GetMessage/${messageId}`
    return apiGetWithAuth(path)
}

export function apiSendMessage(receiverUser, message) {
    const path = "/messages/SendMessage"
    const data = {
        receiverUser,
        message
    }
    return apiPostWithAuth(path, data)
}

async function apiPost(path, data) {
    return postBase(
        path,
        JSON.stringify(data),
        {
            'Content-Type': 'application/json'
        })
}




async function apiGetWithAuth(path) {
    const accessToken = getAccessToken()
    if (!accessToken) {
        return null
    }
    return getBase(
        path,
        {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        })
}

async function apiPostWithAuth(path, data) {
    const accessToken = getAccessToken()
    if (!accessToken) {
        return null
    }
    return postBase(
        path,
        JSON.stringify(data),
        {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        })
}





async function getBase(path, headers) {
    // console.log('path',path);
    const resource = apiUrl + path
    const response = await fetch(resource, {
        method: 'GET',
        headers: headers
    })
    const result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else if (response.status == 401) {
        throw new Error("Unauthorized")
    } else {
        const err = JSON.stringify(result, null, 2)
        //console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}

async function postBase(path, body, headers) {
    const resource = apiUrl + path
    const response = await fetch(resource, {
        method: 'POST',
        headers: headers,
        body: body
    })
    const result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else if (response.status == 401) {
        throw new Error("Unauthorized")
    } else {
        const err = JSON.stringify(result, null, 2)
        //console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}
