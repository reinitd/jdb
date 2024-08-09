package jdb

import (
	"errors"
	"os"
)

type JDB struct {
	Path   string
	isOpen bool
	Tables []string
}

func NewJDB(path string) (*JDB, error) {
	err := initializeFile(path)
	if err != nil {
		return nil, err
	}

	return &JDB{
		Path:   path,
		isOpen: false,
		Tables: []string{"default"},
	}, nil
}

func NewJDBWithDefinedTables(path string, tables []string) (*JDB, error) {
	err := initializeFile(path)
	if err != nil {
		return nil, err
	}

	return &JDB{
		Path:   path,
		isOpen: false,
		Tables: tables,
	}, nil
}

func initializeFile(path string) error {
	if _, err := os.Stat(path); errors.Is(err, os.ErrNotExist) {
		f, err := os.Create(path)
		if err != nil {
			return err
		}
		defer f.Close()

		_, err = f.WriteString("")
		if err != nil {
			return err
		}
		if err := f.Sync(); err != nil {
			return err
		}
	}
	return nil
}
