<script>
    import { apiRegister } from "../../scripts/api-helper";
    import { Router, Link, Route } from "svelte-routing";
    let username = "";
    let password = "";
    let email = "";
    let displayName = "";
    async function register() {
        //console.log(`username ${username}, password : ${password}`);
        var res = await apiRegister(displayName,email, username, password);
        console.log("api result : ", JSON.stringify(res, null, 2));
    }
</script>

<div class="main">
    <h1>Welcome to Chat-App</h1>
    <h2>Register to Chat-App for messaging</h2>
    <input bind:value={username} placeholder="username" /> <br />
    <input bind:value={displayName} placeholder="display name" /> <br />
    <input bind:value={email} placeholder="email" type="email" /> <br />
    <input bind:value={password} placeholder="password" type="password" /><br />
         <button on:click={register}>Register</button>    
        <Link to="login">Login</Link>
 </div>

<style>
     .main {
        text-align: center;
        padding: 1em;
        max-width: 240px;
        margin: 0 auto;
    }
    input, button{
        width: 100%;
        border-radius: .5em ;
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
