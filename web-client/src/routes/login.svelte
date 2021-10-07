<script>
    import { apiLogin } from "../../scripts/api-helper";
    import { authStore } from "../store";
    import { Router, Link, Route } from "svelte-routing";
    import { getNotificationsContext } from "svelte-notifications";

    import { createEventDispatcher, onMount } from "svelte";

    const dispatch = createEventDispatcher();

    onMount(()=>{
        if($authStore.length>0){
            dispatch("successfulLogin");
        }
    })

    const { addNotification } = getNotificationsContext();
    let username = "";
    let password = "";
    async function login() {
        //console.log(`username ${username}, password : ${password}`);
        if (username.length < 4) {
            addNotification({
                text: "username required",
                type: "danger",
                position: "bottom-center",
                removeAfter: 2000,
            });
            return;
        }
        if (password.length < 4) {
            addNotification({
                text: "password required",
                type: "danger",
                position: "bottom-center",
                removeAfter: 2000,
            });
            return;
        }
        try {
            var res = await apiLogin(username, password);
            console.log("api result : ", JSON.stringify(res, null, 2));
            authStore.set(res.accessToken);
            dispatch("successfulLogin");
        } catch (error) {
            addNotification({
                text: error.toString(),
                type: "danger",
                position: "bottom-center",
                removeAfter: 2000,
            });
        }
    }
</script>

<div class="main">
    <h1>Welcome to Chat-App</h1>
    <h2>Login to chat</h2>
    <input bind:value={username} placeholder="username" /> <br />
    <input bind:value={password} placeholder="password" type="password" /><br />
    <button on:click={login}>Login</button> <br />
    <Link to="register">Register</Link>
</div>
<h6>{$authStore}</h6>

<style>
    .main {
        text-align: center;
        padding: 1em;
        max-width: 240px;
        margin: 0 auto;
    }
    input,
    button {
        width: 100%;
        border-radius: 0.5em;
    }
    h1 {
        color: #ff3e00;
        text-transform: uppercase;
        font-size: 2em;
        font-weight: 100;
    }

    @media (max-width: 640px) {
        .main {
            max-width: none;
        }
    }
</style>
