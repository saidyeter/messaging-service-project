const apiUrl = "http://localhost:4000"

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


export function apiGetOlderMessagesFrom(accessToken, messageId) {
    const path = `/Messages/GetOlderMessagesFrom/${messageId}`
    return apiGetWithAuth(path, accessToken)
}

export function apiGetSingleMessage(accessToken, messageId) {
    const path = `/Messages/GetMessage/${messageId}`
    return apiGetWithAuth(path, accessToken)
}

export function apiSendMessage(accessToken,receiverUser, message) {

    const path = "/messages/SendMessage"
    const data = {
        receiverUser,
        message
    }
    return apiPostWithAuth(path,accessToken, data)
}

async function callApi(path, data, verb) {
    const resource = apiUrl + path
    var response = await fetch(resource, {
        method: verb, // *GET, POST, PUT, DELETE, etc.
        // mode: 'cors', // no-cors, *cors, same-origin
        // cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        // credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json'
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        // redirect: 'follow', // manual, *follow, error
        // referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data)
    })
    var result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else {
        const err = JSON.stringify(result, null, 2)
        //console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}





async function apiPost(path, data) {
    return callApi(path, data, 'POST')
}



async function apiGet(path) {
    const resource = apiUrl + path
    var response = await fetch(resource, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
            // 'Content-Type': 'application/x-www-form-urlencoded',
        }
    })
    var result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else {
        const err = JSON.stringify(result, null, 2)
        console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}


async function apiGetWithAuth(path, accessToken) {
    const resource = apiUrl + path
    var response = await fetch(resource, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        }
    })
    var result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else {
        const err = JSON.stringify(result, null, 2)
        console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}

async function apiPostWithAuth(path, accessToken, data) {
    const resource = apiUrl + path
    var response = await fetch(resource, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken,
        },
        body:JSON.stringify(data)
    })
    var result = await response.json()
    if (response.status == 200 || response.status == 201) {
        return result
    } else {
        const err = JSON.stringify(result, null, 2)
        console.error("Error on api: ", err);
        throw new Error(result.error)
    }
}