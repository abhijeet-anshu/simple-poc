/// Search for a pattern in a file and display the lines that contain it.
use clap::Parser;
use std::fs::File;
use std::io::{self, BufRead};


use windows::Win32::System::Threading::*;
use windows::Win32::System::ProcessStatus::{PROCESS_MEMORY_COUNTERS, GetProcessMemoryInfo};

#[derive(Parser)]
struct Cli {
    /// The pattern to look for
    pattern: String,
    /// The path to the file to read
    path: std::path::PathBuf,
}

fn get_memory_usage() -> u64 {
    unsafe {
        let process = GetCurrentProcess();
        let mut mem_counters = PROCESS_MEMORY_COUNTERS::default();
        if GetProcessMemoryInfo(
            process,
            &mut mem_counters as *mut _ as *mut _,
            std::mem::size_of::<PROCESS_MEMORY_COUNTERS>() as u32,
        )
        .as_bool()
        {
            mem_counters.WorkingSetSize as u64
        } else {
            0
        }
    }
}

fn process_file_without_buffer(args: &Cli) {

    let memory_usage_before = get_memory_usage() / 1024;

    println!("Memory usage: {} kbs", memory_usage_before);

    let content = std::fs::read_to_string(&args.path).expect("could not read file");

    let mut line_count = 0;

    for line in content.lines() {
        if line.contains(&args.pattern) {
            line_count += 1;
        }
    }

    println!("Total lines: {}", line_count);

    let memory_usage_after = get_memory_usage() / 1024;
    println!("Memory usage after processing: {} kbs", memory_usage_after);
    println!(
        "Memory usage difference: {} kbs",
        memory_usage_after - memory_usage_before
    );
}

fn process_file(args: &Cli) {
    let file = File::open(&args.path).expect("could not open file");
    let reader = io::BufReader::new(file);

    let memory_usage_before = get_memory_usage() / 1024;

    println!("Memory usage: {} kbs", memory_usage_before);

    let mut line_count = 0;

    for line in reader.lines() {
        let line: String = line.expect("could not read line");
        if line.contains(&args.pattern) {
            line_count += 1;
        }
    }

    println!("Total lines: {}", line_count);

    let memory_usage_after = get_memory_usage() / 1024;
    println!("Memory usage after processing: {} kbs", memory_usage_after);
    println!(
        "Memory usage difference: {} kbs",
        memory_usage_after - memory_usage_before
    );
}

fn main() {
    let args = Cli::parse();
    process_file(&args);
    process_file_without_buffer(&args);
}

