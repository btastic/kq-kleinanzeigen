from time import sleep
import os
from datetime import datetime

from selenium import webdriver
from selenium.webdriver.remote.webelement import WebElement
from selenium.webdriver.common.by import By
from selenium_stealth import stealth

from tqdm.auto import tqdm
import requests
from numpy import random

from Status import Status
from api_requests import CreateItemRequest, CreateSellerRequest, UpdateItemRequest
from Rating import Rating
from Friendliness import Friendliness
from Reliability import Reliability


# Set the path to the Chromedriver
DRIVER_PATH = './chromedriver.exe'


class Scraper:
    """The scraper class"""
    def __init__(self):
        self.driver = None

    def parse_rating(self, rating_element: WebElement) -> Rating:
        """Parses the rating element"""
        if rating_element is None:
            return None

        match rating_element.text:
            case 'NA JA':
                return Rating.NAJA
            case 'OK Zufriedenheit':
                return Rating.OK
            case 'TOP Zufriedenheit':
                return Rating.TOP

    def parse_commercial_seller(self, text: str) -> bool:
        """Parses whether the item was posted by a commercial seller"""
        if text == 'Privater Nutzer':
            return False
        return True

    def parse_reliability(self, reliability_element: WebElement) -> Reliability:
        """Parses the reliability element"""
        if reliability_element is None:
            return None

        match reliability_element.text:
            case 'Zuverlässig':
                return Reliability.RELIABLE
            case 'Sehr zuverlässig':
                return Reliability.VERY_RELIABLE
            case 'Besonders zuverlässig':
                return Reliability.PARTICULARLY_RELIABLE

    def parse_friendliness(self, friendliness_element: WebElement) -> Friendliness:
        """Parses the friendliness element"""
        if friendliness_element is None:
            return None

        match friendliness_element.text:
            case 'Freundlich':
                return Friendliness.FRIENDLY
            case 'Sehr freundlich':
                return Friendliness.VERY_FRIENDLY
            case 'Besonders freundlich':
                return Friendliness.PARTICULARLY_FRIENDLY

    def parse_date_text(self, text: str) -> datetime:
        """Parses date text to datetime"""
        date_text = text.split(' ')[-1]
        return datetime.strptime(date_text, "%d.%m.%Y").date()

    def parse_price(self, text: str) -> float:
        """Parses price text to float"""
        if text == 'Zu verschenken':
            return 0

        text = text.replace('€', '')
        text = text.replace('VB', '')
        text = text.strip()

        if text == '':
            return 0

        return float(text)

    def try_find_element(self, by: str, value: str) -> WebElement | None:
        """Find element, wrapped in try-catch. Used for elements that could not be on the page"""
        try:
            return self.driver.find_element(by, value)
        except:
            return None

    def parse_item_status(self) -> Status:
        """Parses the item status. Available, reserved or deleted"""
        item_deleted = False
        item_reserved = False

        if self.try_find_element(By.XPATH, '//*[text() = "Gelöscht"]') is not None:
            item_deleted = self.try_find_element(
                By.XPATH, '//*[text() = "Gelöscht"]').is_displayed()

        if self.try_find_element(By.XPATH, '//*[text() = "Reserviert"]') is not None:
            item_reserved = self.try_find_element(
                By.XPATH, '//*[text() = "Reserviert"]').is_displayed()

        item_status = Status.AVAILABLE

        if item_reserved:
            item_status = Status.RESERVED
        if item_deleted:
            item_status = Status.DELETED

        return item_status

    def parse_item_images(self, item_gallery_items: list[WebElement]) -> list[str]:
        """Parses the item_gallery WebElement and returns a list of the image urls"""
        item_image_uris = []
        for gallery_item in item_gallery_items:
            try:
                image_element = gallery_item.find_element(By.XPATH, 'meta[4]')
                item_image_uris.append(image_element.get_attribute('content'))
            except:
                pass

        return item_image_uris

    def parse_xpath_text(self, xpath: str) -> str:
        """Short hand to get the text from a given XPath"""
        return self.driver.find_element(By.XPATH, xpath).text

    def parse_seller(self) -> CreateSellerRequest:
        """Parses the seller on the item page"""
        seller_name = self.parse_xpath_text(
            '//*[@id="viewad-contact"]/div/ul/li/span/span[1]/a')

        seller_id = self.driver.find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/span[1]/a').get_attribute(
            'href').rsplit('?', 1)[-1].replace('userId=', '')

        seller_rating = self.parse_rating(
            self.try_find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/div/span/span'))

        seller_friendliness = self.parse_friendliness(
            self.try_find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/div/span[2]/span'))

        seller_reliability = self.parse_reliability(
            self.try_find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/div/span[3]/span'))

        seller_commercial_seller = self.parse_commercial_seller(
            self.driver.find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/span[2]/span').text)

        seller_active_since = self.parse_date_text(
            self.driver.find_element(By.XPATH, '//*[@id="viewad-contact"]/div/ul/li/span/span[3]/span').text)

        return CreateSellerRequest(
            seller_id,
            seller_name,
            seller_rating,
            seller_friendliness,
            seller_reliability,
            seller_active_since,
            seller_commercial_seller)

    def parse_item(self) -> CreateItemRequest:
        """Parses an item"""
        seller = self.parse_seller()

        item_id = int(self.driver.find_element(
            By.XPATH, '//*[@id="viewad-ad-id-box"]/ul/li[2]').text)

        item_category = self.parse_xpath_text(
            '//*[@id="vap-brdcrmb"]/a[3]/span')

        item_title = self.parse_xpath_text('//*[@id="viewad-title"]')

        item_price = self.parse_price(
            self.driver.find_element(By.XPATH, '//*[@id="viewad-price"]').text)

        if self.try_find_element(By.XPATH, '//*[@id="viewad-main-info"]/div[1]/span') is not None:
            item_shipping = self.driver.find_element(
                By.XPATH, '//*[@id="viewad-main-info"]/div[1]/span').text
        else:
            item_shipping = None

        item_locality = self.parse_xpath_text('//*[@id="viewad-locality"]')
        item_zip_code = item_locality.split(' ')[0]
        item_city = item_locality.split(' ')[1]

        item_posted_at = self.parse_date_text(
            self.driver.find_element(By.XPATH, '//*[@id="viewad-extra-info"]/div[1]/span').text)

        item_description = self.parse_xpath_text(
            '//*[@id="viewad-description-text"]')

        item_gallery_items = self.driver.find_elements(
            By.CLASS_NAME, 'galleryimage-element')

        item_image_uris = self.parse_item_images(item_gallery_items)

        item_status = self.parse_item_status()

        create_item_request = CreateItemRequest(
            item_id,
            seller,
            item_title,
            item_description,
            item_price,
            item_shipping,
            item_zip_code,
            item_city,
            item_category,
            item_status,
            item_posted_at,
            item_image_uris)

        return create_item_request

    def post_item(self, item: CreateItemRequest):
        """Creates an item on the server"""
        requests.post('https://kqka.ds007.myds.me/items',
                      json=item.to_json(), timeout=20)

    def update_item(self, item_id: int, item: UpdateItemRequest):
        """Updates an item on the server"""
        requests.patch(
            f'https://kqka.ds007.myds.me/items/{item_id}', json=item.to_json(), timeout=20)

    def post_exists(self, item_id: int) -> bool:
        """Checks whether a post exists on the server"""
        r = requests.get(
            f'https://kqka.ds007.myds.me/items/{item_id}', timeout=20)
        return r.status_code == 200

    def run(self, update: bool = False):
        """Run the scraper"""
        options = webdriver.ChromeOptions()
        options.add_argument("start-maximized")

        # options.add_argument("--headless")

        options.add_experimental_option(
            "excludeSwitches", ["enable-automation", "enable-logging"])
        options.add_experimental_option('useAutomationExtension', False)
        options.add_argument(f'load-extension={os.getcwd()}/1.60.0_0')

        chrome_service = webdriver.ChromeService(
            executable_path=r'chromedriver.exe')
        self.driver = webdriver.Chrome(options=options, service=chrome_service)

        stealth(self.driver,
                languages=["en-US", "en"],
                vendor="Google Inc.",
                platform="Win32",
                webgl_vendor="Intel Inc.",
                renderer="Intel Iris OpenGL Engine",
                fix_hairline=True)

        if update:
            self.update_items()
        else:
            self.scrape_items()

        self.driver.quit()

    def update_items(self):
        """Updates existing items on the server"""
        r = requests.get('https://kqka.ds007.myds.me/items', timeout=20)
        assert r.status_code == 200
        for item in tqdm(r.json(), position=0, leave=True):
            try:
                item_details = self.scrape_item(item["id"])
                self.update_item(item_details.item_id, UpdateItemRequest(
                    item_details.title, item_details.description, item_details.price, item_details.status))
            except Exception as error:
                print(f'Unable to parse {item["id"]}: {error}')

            sleeptime = random.uniform(2, 10)
            sleep(sleeptime)

    def scrape_items(self):
        """Scrapes and creates new items on the server"""
        for i in tqdm(range(1, 10), position=0, leave=True):
            item_ids = self.scrape_item_urls_from_listing(i, 'fpv')

            for item_id in tqdm(item_ids, position=0, leave=True):
                try:
                    item_details = self.scrape_item(item_id)
                    self.post_item(item_details)
                except Exception as error:
                    print(f'Unable to parse {item_id}: {error}')
                sleeptime = random.uniform(2, 10)
                sleep(sleeptime)

    def scrape_item_urls_from_listing(self, page: int, keyword: str) -> list[int]:
        """Scrapes a listing page with 0-25 items and returns a list of their ids"""
        self.driver.get(
            f'https://www.kleinanzeigen.de/s-seite:{str(page)}/{keyword}/k0')
        item_ids = []
        for listing in self.driver.find_elements(By.XPATH, '//*[@id="srchrslt-adtable"]/li'):
            try:
                element = listing.find_element(By.TAG_NAME, 'article')
                item_id = element.get_attribute('data-adid')
                if self.post_exists(item_id):
                    print("Item already exists, skipping ...")
                    continue
                item_ids.append(item_id)
            except:
                # some of the <li> tags are ads, hence no <article> tag will be found
                pass

        return item_ids

    def scrape_item(self, item_id: str) -> CreateItemRequest:
        """Scrapes the details about an item"""
        self.driver.get(f'https://www.kleinanzeigen.de/s-anzeige/{item_id}')
        item_details = self.parse_item()

        return item_details


scraper = Scraper()
scraper.run(update=False)
