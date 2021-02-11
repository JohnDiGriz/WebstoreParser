# Website Parser
Parses data from websites using json file as instruction and writes them to SQL server database.
Parsed data is a list of products with connected pictures and prices. Multiple runs on same website update existing records, including keeping price history

# Data Format
Note that Foreign and Primary keys, as well as navigation properties are ommited
Product:
* Url
* Name
* Current Price
* Description
* Url of header image
* List of prices (price history)
* List of images (other, non header images)
Price:
* Value
* Date and time of parsing
Picture:
* Url

# Parser Rules Format
Parser Rules is json file that contains mapping of ParserRules object. It takes form of series of (mostly) xpathes to various places on webpages, where data for products is stored
Rules:
* HomePage - Url of page, from which parser starts crawling
* DetailsXpath - Xpath to get url of product from page
* PaginationXpath - Xpath to get links to other pages
* PaginationPattern - Regex pattern of page numeration part of link, needed to get page number
* MaxPageValue - maximal page number to crawl
* HeaderXpath - Xpath to get product name from product page
* PriceXpath - Xpath to get product price from product page
* DescriptionXpath - Xpath to get product description from product page
* PicturesXpath - Xpath to get url of product images from product page

# Program args
Program requires three arguments to run: path to directory with parser files (-d %directory%), delay between runs of the same parser (-t %delay in hours%)
and delay betweem update on the parser directory (-u %update delay in minutes%)
Program can run indefinately, regularly updating it's parser base
