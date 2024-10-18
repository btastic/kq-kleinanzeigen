from selenium import webdriver
from selenium_stealth import stealth
from selenium.webdriver.common.by import By
from numpy import random
from time import sleep

sleeptime = random.uniform(2, 4)

# Set the path to the Chromedriver
DRIVER_PATH = './chromedriver.exe'

class Scraper:
    def parse_item(item_url):
        # TODO: scrape and parse html from item listing (example item_url: https://www.kleinanzeigen.de/s-anzeige/2895626073)
        return
    
    def parse_seller(seller_url):
        # TODO: scrape and parse html from seller (example seller_url: https://www.kleinanzeigen.de/s-bestandsliste.html?userId=92078547)
        return

    def run(self):
        options = webdriver.ChromeOptions()
        options.add_argument("start-maximized")

        # options.add_argument("--headless")

        options.add_experimental_option("excludeSwitches", ["enable-automation"])
        options.add_experimental_option('useAutomationExtension', False)

        chromeService = webdriver.ChromeService(executable_path=r'chromedriver.exe')
        self.driver = webdriver.Chrome(options=options, service = chromeService)

        stealth(self.driver,
                languages=["en-US", "en"],
                vendor="Google Inc.",
                platform="Win32",
                webgl_vendor="Intel Inc.",
                renderer="Intel Iris OpenGL Engine",
                fix_hairline=True)
        
        self.scrape()
        self.driver.quit()

    def scrape_item_urls_from_listing(self, page, keyword):
        self.driver.get('https://www.kleinanzeigen.de/s-seite:' + page + '/' + keyword + '/k0')
        itemUrls = []
        for listing in self.driver.find_elements(By.XPATH, '//*[@id="srchrslt-adtable"]/li'):
            try:
                element = listing.find_element(By.TAG_NAME, 'article')
                itemUrls.append('https://www.kleinanzeigen.de/s-anzeige/' + element.get_attribute('data-adid'))
            except:
                # some of the <li> tags are ads, hence no <article> tag will be found
                pass

        return itemUrls
    
    def scrape_seller(self, user_url):
        self.driver.get(user_url)

        return self.parse_seller()

    def scrape_item(self, item_url):
        self.driver.get(item_url)
        seller_url = self.driver.find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/span[1]/a')
        seller_details = self.scrape_seller(seller_url)
        item_details = self.parse_item()

        return (seller_details, item_details)

    def scrape(self):
        item_urls = self.scrape_item_urls_from_listing(page = 1, keyword='fpv')

        for url in item_urls:
            item_details = self.scrape_item(url)
            sleeptime = random.uniform(2, 10)
            sleep(sleeptime)
            # TODO: send item_details to backend API

scraper = Scraper()
scraper.run()