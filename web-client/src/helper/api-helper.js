const apiUrl ="http://ms-nginx:4000"//"http://localhost:4000"

export function apiLogin(username, password) {

    const path = "/auth/login"
    const data = {
        username,
        password
    }
    return apiPost(path, data)
}

export function apiRegister(displayname, email, username, password) {

    const path = "/auth/register"
    const data = {
        username,
        password,
        displayname,
        email
    }
    return apiPost(path, data)
}

export function apiGetOpponents(accessToken) {
    const path = `/Messages/GetOpponents`
    return apiGetWithAuth(path, accessToken)
}

export function apiGetLastMessageId(accessToken, opponent) {
    const path = `/Messages/GetLatestMessageBetween/${opponent}`
    return apiGetWithAuth(path, accessToken)
}

export function apiGetUserInfo(accessToken, opponent) {
    const path = `/Messages/GetUserInfo/${opponent}`
    return apiGetWithAuth(path, accessToken)
}

export function apiGetOlderMessagesFrom(accessToken, messageId) {
    const path = `/Messages/GetOlderMessagesFrom/${messageId}`
    return apiGetWithAuth(path, accessToken)
}

export function apiGetSingleMessage(accessToken, messageId) {
    const path = `/Messages/GetMessage/${messageId}`
    return apiGetWithAuth(path, accessToken)
}

export function apiSendMessage(accessToken, receiverUser, message) {
    const path = "/messages/SendMessage"
    const data = {
        receiverUser,
        message
    }
    return apiPostWithAuth(path, accessToken, data)
}

async function apiPost(path, data) {
    return postBase(
        path,
        JSON.stringify(data),
        {
            'Content-Type': 'application/json'
        })
}




async function apiGetWithAuth(path, accessToken) {
    return getBase(
        path,
        {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        })
}

async function apiPostWithAuth(path, accessToken, data) {
    return postBase(
        path,
        JSON.stringify(data),
        {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        })
}


async function getBase(path, headers) {
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
