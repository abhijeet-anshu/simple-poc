/// Search for a pattern in a file and display the lines that contain it.
use clap::Parser;
use std::fs::File;
use std::io::{self, BufRead};

#[derive(Parser)]
struct Cli {
    /// The pattern to look for
    pattern: String,
    /// The path to the file to read
    path: std::path::PathBuf,
}


fn main() {
    let args = Cli::parse();

    let file = File::open(&args.path).expect("could not open file");
    let reader = io::BufReader::new(file);

    for line in reader.lines() {
        let line: String = line.expect("could not read line");
        if line.contains(&args.pattern) {
            println!("{}", line);
        }
    }
}

