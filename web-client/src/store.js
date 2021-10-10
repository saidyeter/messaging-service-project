import { writable } from 'svelte/store'

const cookieStore = (key, initial) => {
    const persist = getCookie(key)
    const data = persist ? JSON.parse(persist) : initial
    //if sub is broken, sets value to current local storage value
    const store = writable(data, () => {
        const unsubscribe = store.subscribe(value => {
            setCookie(key, JSON.stringify(value), 30)
        })
        return unsubscribe
    })
    return store
}
const localStorageStore = (key, initial) => {
    const persist = localStorage.getItem(key)
    const data = persist ? JSON.parse(persist) : initial
    //if sub is broken, sets value to current local storage value
    const store = writable(data, () => {
      const unsubscribe = store.subscribe(value => {
        localStorage.setItem(key, JSON.stringify(value))
      })
      return unsubscribe
    })
    return store
  } 

function setCookie(name, value, minutes) {
    // if (getCookie(name).length > 0) {
    //     return
    // }
    var expires;
    if (minutes) {
        var date = new Date();
        date.setTime(date.getTime() + (minutes * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    }
    else {
        expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function getCookie(c_name) {
    if (document.cookie.length > 0) {
        let c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            let c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) {
                c_end = document.cookie.length;
            }
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}

export const authStore = cookieStore('accessToken', "")
export const messageList = writable([])
export const opponentList = writable([])
export const currentUser = localStorageStore("account","")