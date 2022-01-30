export async function handle({ request, resolve }) {
    console.log("handle",request);
    request.locals.xsy = "said."
    const response = await resolve(request);
    return {
        ...response,
        headers:{
            ...response.headers,
            test: "bidibidi"
        }
    }
}

export function getSession(request) {
    console.log("getSession",request.locals);
    return {
        user: {
            name: "said",
            accessToken: "iuahsdiuahiudhasiudhiuashdiuh"
        }
    }
}