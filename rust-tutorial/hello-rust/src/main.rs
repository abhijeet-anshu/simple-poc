use ferris_says::say; // from the previous step
use std::io::{stdout, BufWriter, Write};

fn main() {
    let stdout = stdout();
    let message = String::from("Hello fellow Rustaceans!");
    let width: usize =  (message.chars().count() as f64 * 4.5) as usize;

    let mut writer = BufWriter::new(stdout.lock());
    match say(&message, width, &mut writer) {
        Ok(_) => {
            writer.flush().unwrap();
            // Successfully wrote the message
            println!("Message written successfully!");
        }
        Err(e) => {
            // Handle the error
            eprintln!("Failed to write the message: {}", e);
        }
    }
}