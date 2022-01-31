<script>
	import { apiLogin } from '$lib/helper/api-helper.js';
	import { currentUser } from '$lib/helper/store';
	import { getNotificationsContext } from 'svelte-notifications';
	const { addNotification } = getNotificationsContext();

	let username = 'said';
	let password = 'ASDqwe123';
	async function login() {
		console.log(`username ${username}, password : ${password}`);
		if (username.length < 4) {
			addNotification({
				text: 'username required',
				type: 'danger',
				position: 'bottom-center',
				removeAfter: 2000
			});
			return;
		}
		if (password.length < 4) {
			addNotification({
				text: 'password required',
				type: 'danger',
				position: 'bottom-center',
				removeAfter: 2000
			});
			return;
		}
		try {
			const res = await apiLogin(username, password);
		
			if (!res) {
				addNotification({
					text: 'couldnt login',
					type: 'danger',
					position: 'bottom-center',
					removeAfter: 2000
				});
				return;
			}
			
			currentUser.set(username);
			
			window.location= "/list"
		} catch (error) {
			addNotification({
				text: error.toString(),
				type: 'danger',
				position: 'bottom-center',
				removeAfter: 2000
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
	<a href="register">register</a>
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
		font-weight: 300;
	}
	h2 {
		font-weight: 300;
	}

	@media (max-width: 640px) {
		.main {
			max-width: none;
		}
	}
</style>
