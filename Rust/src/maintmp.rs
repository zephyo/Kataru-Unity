use kataru::*;

fn main() {
    let path = "C:\\Users\\Joshi\\Dev\\Unity\\Kataru-Unity\\Assets\\Kataru\\bookmark.yml";

    let bookmark = Bookmark::load(path).unwrap();
    println!("len: {}, {:?}", path.len(), bookmark);
}
