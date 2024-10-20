from datetime import datetime

from Friendliness import Friendliness
from Rating import Rating
from Reliability import Reliability
from Status import Status


class CreateSellerRequest:
    """Seller DTO for sending a post request to the API"""

    def __init__(self,
                 seller_id: int,
                 name: str,
                 rating: Rating,
                 friendliness: Friendliness,
                 reliability: Reliability,
                 active_since: datetime,
                 commercial_seller: bool):
        self.seller_id = seller_id
        self.name = name
        self.rating = rating
        self.friendliness = friendliness
        self.reliability = reliability
        self.active_since = active_since
        self.commercial_seller = commercial_seller


class UpdateItemRequest:
    """Item DTO for sending a patch request to the API"""

    def __init__(self, title: str, description: str, price: float, status: Status):
        self.title = title
        self.description = description
        self.price = price
        self.status = status

    def to_json(self):
        """Converts the item to a dict"""
        return {
            "title": self.title,
            "description": self.description,
            "price": self.price,
            "status": self.status.value,
        }


class CreateItemRequest:
    """Item DTO for sending a post request to the API"""

    def __init__(self,
                 item_id: int,
                 seller: CreateSellerRequest,
                 title: str,
                 description: str,
                 price: float,
                 shipping: str,
                 zip_code: str,
                 city: str,
                 category: str,
                 status: Status,
                 posted_at: datetime,
                 image_uris: list[str]):
        self.item_id = item_id
        self.seller = seller
        self.title = title
        self.description = description
        self.price = price
        self.shipping = shipping
        self.zip_code = zip_code
        self.city = city
        self.category = category
        self.status = status
        self.posted_at = posted_at
        self.image_uris = image_uris

    def to_json(self):
        """Converts the item to a dict"""
        return {
            "id": self.item_id,
            "seller": {
                "id": self.seller.seller_id,
                "name": self.seller.name,
                "rating": self.seller.rating.value if self.seller.rating is not None else None,
                "friendliness": self.seller.friendliness.value if self.seller.friendliness is not None else None,
                "reliability": self.seller.reliability.value if self.seller.reliability is not None else None,
                "activeSince": self.seller.active_since.strftime('%Y-%m-%d'),
                "commercialSeller": self.seller.commercial_seller
            },
            "imageUris": self.image_uris,
            "title": self.title,
            "description": self.description,
            "price": self.price,
            "shipping": self.shipping,
            "zipCode": self.zip_code,
            "city": self.city,
            "category": self.category,
            "status": self.status.value,
            "postedAt": self.posted_at.strftime('%Y-%m-%d')
        }
