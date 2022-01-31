<script>
	import { apiRegister } from '$lib/helper/api-helper.js';
	let username = '';
	let password = '';
	let email = '';
	let displayName = '';
	async function register() {
		//console.log(`username ${username}, password : ${password}`);
		var res = await apiRegister(displayName, email, username, password);
		if(res){
			window.location= "/login"
		}
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
	<a href="login">login</a>
</div>

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
