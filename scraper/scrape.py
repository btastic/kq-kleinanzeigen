from selenium import webdriver
from selenium_stealth import stealth

# Set the path to the Chromedriver
DRIVER_PATH = './chromedriver.exe'


options = webdriver.ChromeOptions()
options.add_argument("start-maximized")

# options.add_argument("--headless")

options.add_experimental_option("excludeSwitches", ["enable-automation"])
options.add_experimental_option('useAutomationExtension', False)

chromeService = webdriver.ChromeService(executable_path=r'chromedriver_patched.exe')
driver = webdriver.Chrome(options=options, service = chromeService)

stealth(driver,
        languages=["en-US", "en"],
        vendor="Google Inc.",
        platform="Win32",
        webgl_vendor="Intel Inc.",
        renderer="Intel Iris OpenGL Engine",
        fix_hairline=True,
        )

# Navigate to the URL
driver.get('https://www.kleinanzeigen.de/s-fpv/k0')

# It's a good practice to close the browser when done
driver.quit()