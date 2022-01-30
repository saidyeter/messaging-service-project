import * as cookie from "cookie";
import { apiLogin } from '$lib/helper/api-helper.js';

export async function post(request) {
    let { username, password } = JSON.parse(request.body)
    var res = await apiLogin(username, password);
    const headers = {
        "set-cookie": cookie.serialize("mahmut", res.accessToken, {
            httpOnly: true,
            sameSite: "lax",
            secure: true,
            maxAge: 60 * 60 * 24,
            path: "/"
        }),
    }
    console.log("headers",headers);
    return {
        status: 200,
        headers,
        body: {
            message: "success",
        }
    }

}