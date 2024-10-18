class Item:
    def __init__(self, id, seller_id, title, description, price, shipping_cost, views, zip_code, city, category, created_at, image_uris):
        self.id = id
        self.seller_id = seller_id
        self.title = title
        self.description = description
        self.price = price
        self.shipping_cost = shipping_cost
        self.views = views
        self.zip_code = zip_code
        self.city = city
        self.category = category
        self.created_at = created_at
        self.image_uris = image_uris