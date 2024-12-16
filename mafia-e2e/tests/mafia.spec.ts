import {test} from "@playwright/test";

test('game test scenario', async ({ page }) => {
  await page.goto('/');

  const usernameInput = await page.waitForSelector('[data-test-id="username-field"]', { state: 'attached' });
  const loginButton = await page.waitForSelector('[data-test-id="login-button"]', {state: 'attached'});

  const timestamp = Date.now();
  const delay = Math.ceil(Math.random() * 3000);

  await page.waitForTimeout(delay);

  const username = 'TestUser-'+timestamp+delay;
  await usernameInput.fill(username);
  await page.waitForTimeout(Math.ceil(Math.random() * 500));
  await loginButton.click();

  await page.waitForTimeout(4000);
  // const startButton = page.locator('[data-test-id="start-game-button"]');
  let isHost = false;
  try {
    const startButton = await page.waitForSelector('[data-test-id="start-game-button"]', {state: 'attached', timeout: 4000});
    isHost = await startButton.isVisible();

    if (isHost) {
      await startButton.click();
    }
  } catch {
    isHost = false;
  }

  // await page.waitForTimeout(6000);

  const roleElement = page.locator('[data-test-id="role-name"]');
  const roleName = await roleElement.innerText({timeout: 10000});

  console.log(roleName);

  const chatInput = await page.waitForSelector('[data-test-id="chatbox-input"]', {state: 'attached', timeout: 1000});
  const chatButton = await page.waitForSelector('[data-test-id="chatbox-button"]', {state: 'attached', timeout: 1000});

  await chatInput.fill("Hello, world from " + username);
  await page.waitForTimeout(Math.ceil(Math.random() * 1000));
  await chatButton.click();

  await page.waitForTimeout(5000);
});